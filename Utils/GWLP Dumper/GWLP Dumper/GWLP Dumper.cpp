#include "GWLP Dumper.h"
#include "MySQLDatabase.h"
#include <iostream>
#include <sstream>
#include <string>
#include <map>
#include <queue>
#include "tinyxml.h"

using namespace std;
HANDLE hMutex;
map<DWORD,handler> PacketHandler;
queue<Packet*> PacketQueue;

#pragma region Util

MySQLDatabase* Database = NULL;
template<typename T> T Get(BYTE* start)
{
	T val;
	__try
	{
		memcpy(&val,start,sizeof(T));
	}
	__except(1)
	{
		return val;
	}
	return val;
}

inline void AddHandler( DWORD head,handler func)
{
	PacketHandler[head] = func;
}

bool InitDumper()
{
	TiXmlDocument config(CONFIG_NAME);
	if(!config.LoadFile())
	{
		DisplayError("Config Error: %s",config.ErrorDesc());
		return false;
	}
	TiXmlNode* root = config.RootElement();
	if(!root)
	{
		DisplayError("Config Error: Config file empty!");
		return false;
	}
	
	const char *host,*db,*user,*pw,*tmpport;
	int port;
	TiXmlHandle handle(root);
	TiXmlElement* element;
	/**
	* <DumperConfig>
	*	<Hostname>localhost</Hostname>
	*	<Database>gwlp</Database>
	*	<User>root</User>
	*	<Password></Password> //optional
	*	<Port></Port>		  //optional
	* </DumperConfig>
	**/
	element = handle.FirstChild("Hostname").ToElement();
	host = element->FirstChild()->Value();
	element = handle.FirstChild("Database").ToElement();
	db = element->FirstChild()->Value();
	element = handle.FirstChild("User").ToElement();
	user = element->FirstChild()->Value();
	element = handle.FirstChild("Password").ToElement();
	if(element)
	{
		if(element->FirstChild())
		{
			pw = element->FirstChild()->Value();
		}
		else
		{
			pw = "";
		}	
	}
	else
	{
		pw = "";
	}
	element = handle.FirstChild("Port").ToElement();
	if(element)
	{
		if(element->FirstChild())
		{
			tmpport = element->FirstChild()->Value();
			port = atoi(tmpport);
		}
		else
		{
			port = 0;
		}
	}
	else
	{
		port = 0;
	}
	try
	{
		Database = new MySQLDatabase(host,db,user,pw,port);;
	}
	catch (const char* error)
	{
		DisplayError("MYSQL Error: %s",error);
		return false;
	}

	AddHandler(0x0185,ProcessLoadSpawnPoint);
	AddHandler(0x0189,ProcessLoadCharName);
	AddHandler(0x004A,ProcessNPCPacket);
	AddHandler(0x004B,ProcessNPCGraphics);
	AddHandler(0x008E,ProcessNPCName);
	AddHandler(0x0015,ProcessNPCSetMiscCape);

	hMutex = CreateMutex(NULL,FALSE,TEXT("PacketMutex"));
	if(!hMutex)
	{
		DisplayError("Unable to create mutex!");
		return false;
	}
	CreateThread(0,0,(LPTHREAD_START_ROUTINE)ProcessPacketThread,0,0,0);
	return true;
}

void CloseDumper()
{
	if(Database)
		Database->~MySQLDatabase();
}

DWORD GetPacketLength( DWORD head,BYTE* pack )
{
	switch(head)
	{
	case 0x0185:
		return 14;
		break;
	case 0x0189:
		return 7;
		break;
	case 0x004A:
		return (Get<short>(pack + 26) * 2) + 28;
		break;
	case 0x004B:
		return 10;
		break;
	case 0x008E:
		return (Get<short>(pack + 4) * 2) + 6;
		break;
	case 0x0015:
		return 35;
		break;
	}
	return 0;
}

void EnqueuePacket(Packet* pack)
{
	if(WaitForSingleObject(hMutex,INFINITE) == WAIT_OBJECT_0)
	{
		PacketQueue.push(pack);
		ReleaseMutex(hMutex);
	}
	else
	{
		DisplayError("Unable to retrieve Mutex!");
	}
}

void __stdcall ProcessPacketThread()
{
	while(true)
	{	
		if(WaitForSingleObject(hMutex,INFINITE) == WAIT_OBJECT_0)
		{
			if(PacketQueue.empty())
			{
				ReleaseMutex(hMutex);
				continue;
			}
			Packet* pack = PacketQueue.front();
			PacketQueue.pop();
			ReleaseMutex(hMutex);

			map<DWORD,handler>::iterator it;
			it = PacketHandler.find(pack->Header);
			if(it != PacketHandler.end())
			{
				it->second(pack->pData);
			}
			if(pack->pData)
				delete[] pack->pData;

			delete[] pack;
		}
		else
		{
			DisplayError("Unable to retrieve Mutex!");
		}
		Sleep(50);
	}
}

#pragma endregion

#pragma region ZoneData

short LastZoneID = 0;
bool isOutpost = false;

void ProcessLoadCharName(BYTE* pack)
{
	short ZoneID;
	BYTE unk;
	int curPos = 4;
	ZoneID = Get<short>(pack + curPos);curPos+=2;
	unk = Get<BYTE>(pack + curPos);

	LastZoneID = ZoneID;
	isOutpost = (unk == 0);

	MySQLQueryResult* result = Database->Query("SELECT * FROM map_hashes WHERE ZoneID = %i",ZoneID);
	if(result)
	{
		result->~MySQLQueryResult();
		return;
	}

	Database->Execute("INSERT INTO map_hashes (ZoneID) VALUES (%i)",ZoneID);
	if(Database->LastError()[0] != '\0')
		DisplayError("MYSQL Error: %s",Database->LastError());

	printf("Dumped ZoneID: %i to map_hashes\n",ZoneID);
}

void ProcessLoadSpawnPoint(BYTE* pack)
{
	const char* MapFileHash;
	float SpawnX,SpawnY;
	short SpawnPlane;
	int curPos = 0;

	MapFileHash = ConvertToHex(Get<DWORD>(pack + curPos));curPos+=4;
	SpawnX = Get<float>(pack + curPos);curPos+=4;
	SpawnY = Get<float>(pack + curPos);curPos+=4;
	SpawnPlane = Get<short>(pack + curPos);

	MySQLQueryResult* result = Database->Query("SELECT * FROM map_spawns WHERE MapFileHash='%s' AND SpawnX=%f AND SpawnY=%f AND SpawnPlane=%i",MapFileHash,SpawnX,SpawnY,SpawnPlane);
	if(result)
	{
		result->~MySQLQueryResult();
		return;
	}

	Database->Execute("INSERT INTO map_spawns (MapFileHash,ZoneID,IsOutpost,Flags,SpawnX,SpawnY,SpawnPlane) VALUES ('%s',%i,%i,0,%f,%f,%i)",MapFileHash,LastZoneID,isOutpost?1:0,SpawnX,SpawnY,SpawnPlane);
	if(Database->LastError()[0] != '\0')
		DisplayError("MYSQL Error: %s",Database->LastError());

	printf("Dumped MapFileHash: %s, ZoneID: %i, IsOutpost: %i, SpawnX: %f, SpawnY: %f, SpawnPlane: %i to map_spawns \n",MapFileHash,LastZoneID,isOutpost?1:0,SpawnX,SpawnY,SpawnPlane);

	Database->Execute("UPDATE map_hashes SET MapFileHash='%s' WHERE ZoneID=%i",MapFileHash);
	if(Database->LastError()[0] != '\0')
		DisplayError("MYSQL Error: %s",Database->LastError());
}

#pragma endregion

#pragma region NPCData

BYTE LastLevel,LastProfession;
map<long,string> NPCNames;

void ProcessNPCPacket(BYTE* pack)
{
	long LocalId,NPCId;
	short Scale,LenOfRemainder;
	BYTE ShowProfession,Profession,Level;
	int curPos = 0;
	
	LocalId = Get<long>(pack + curPos);curPos +=4;
	NPCId = Get<long>(pack + curPos);curPos += 4;
			curPos += 7;
	Scale = Get<short>(pack + curPos);curPos += 2;
			curPos += 3;
	ShowProfession = Get<BYTE>(pack + curPos);curPos += 1;
			curPos += 3;
	Profession = Get<BYTE>(pack + curPos);curPos += 1;
	Level = Get<BYTE>(pack + curPos);curPos += 1;
	LenOfRemainder = Get<BYTE>(pack + curPos);curPos +=2;

	LastLevel = Level;
	LastProfession = Profession;

	char tmp[4];
	ZeroMemory(tmp,sizeof(tmp));

	stringstream appearance;
	for(int i = 0;i<LenOfRemainder * 2;i++)
	{
		sprintf(tmp,"%02X",pack[curPos + i]);
		appearance.put(tmp[0]);
		appearance.put(tmp[1]);
	}

	MySQLQueryResult* result = Database->Query("SELECT * FROM npc_data WHERE LocalID=%i",LocalId);
	if(result)
	{
		Database->Execute("UPDATE npc_data SET NPC_ID=%u,Scale=%i,ShowProfession=%i,Appearance='%s' WHERE LocalID=%u",NPCId,Scale,(int)ShowProfession,appearance.str().c_str(),LocalId);
		if(Database->LastError()[0] != '\0')
			DisplayError("MYSQL Error: %s",Database->LastError());
		result->~MySQLQueryResult();

		printf("Dumped NPC_ID: %u, Scale: %i, ShowProfession: %i to npc_data with LocalID: %u\n",NPCId,Scale,(int)ShowProfession,LocalId);
	}
	else
	{
		Database->Execute("INSERT INTO npc_data (LocalID,NPC_ID,Scale,ShowProfession,Appearance) VALUES(%u,%u,%i,%i,'%s')",LocalId,NPCId,Scale,(int)ShowProfession,appearance.str().c_str());
		if(Database->LastError()[0] != '\0')
			DisplayError("MYSQL Error: %s",Database->LastError());

		printf("Dumped LocalID: %u, NPC_ID: %u, Scale: %i, ShowProfession %i to npc_data\n",LocalId,NPCId,Scale,(int)ShowProfession);
	}
}

void ProcessNPCGraphics(BYTE* pack)
{
	long LocalId,ModelHash;
	short Flag;
	int curPos = 0;

	LocalId = Get<long>(pack + curPos);curPos += 4;
	Flag = Get<short>(pack + curPos);curPos +=2;
	ModelHash = Get<long>(pack + curPos);

	MySQLQueryResult* result = Database->Query("SELECT * FROM npc_data WHERE LocalID=%u",LocalId);
	if(result)
	{
		Database->Execute("UPDATE npc_data SET ModelHash=%u,Flag=%i WHERE LocalID=%u",ModelHash,Flag,LocalId);
		if(Database->LastError()[0] != '\0')
			DisplayError("MYSQL Error: %s",Database->LastError());
		result->~MySQLQueryResult();

		printf("Dumped ModelHash: %u, Flag: %i to npc_data with LocalID: %u\n",ModelHash,Flag,LocalId);
	}
	else
	{
		Database->Execute("INSERT INTO npc_data (LocalID,ModelHash,Flag) VALUES(%i,%u,%u)",LocalId,ModelHash,Flag);
		if(Database->LastError()[0] != '\0')
			DisplayError("4MYSQL Error: %s",Database->LastError());

		printf("Dumped LocalID: %u, ModelHash: %u, Flag: %i to npc_data\n",LocalId,ModelHash,Flag);
	}
}

void ProcessNPCName(BYTE* pack)
{
	long AgentID;
	short LenOfRemainder;
	int curPos = 0;

	AgentID = Get<long>(pack + curPos);curPos +=4;
	LenOfRemainder = Get<short>(pack + curPos);curPos +=2;

	char Name[4];
	ZeroMemory(Name,sizeof(Name));

	stringstream query;
	for(int i = 0;i<LenOfRemainder * 2;i++)
	{
		sprintf(Name,"%02X",pack[curPos + i]);
		query.put(Name[0]);
		query.put(Name[1]);
	}

	NPCNames[AgentID] = query.str();
}

void ProcessNPCSetMiscCape(BYTE* pack)
{
	long AgentId;
	short LocalId;
	float X,Y,Speed,Rotation;
	short Plane;
	int curPos = 0;

	AgentId = Get<long>(pack + curPos);curPos +=4;
	LocalId = Get<short>(pack + curPos);curPos +=2;
		curPos += 4;
	X = Get<float>(pack + curPos);curPos +=4;
	Y = Get<float>(pack + curPos);curPos +=4;
	Plane = Get<short>(pack + curPos);curPos +=2;
		curPos += 4;
	Rotation = Get<float>(pack + curPos);curPos +=4;
		curPos += 1;
	Speed = Get<float>(pack + curPos);

	MySQLQueryResult* result = Database->Query("SELECT * FROM npc_data WHERE LocalID=%i",LocalId);
	if(!result)
		return;
	else
		result->~MySQLQueryResult();

	result = Database->Query("SELECT * FROM npc_spawns WHERE LocalID=%i AND SpawnX=%f AND SpawnY=%f AND Plane=%i",LocalId,X,Y,Plane);
	if (!result)
	{
		Database->Execute("INSERT INTO npc_spawns (SpawnId,LocalID,ZoneID,SpawnX,SpawnY,Plane,Rotation,Speed,Level,Profession) VALUES(%i,%u,%i,%f,%f,%i,%f,%f,%i,%i)",0,LocalId,LastZoneID,X,Y,Plane,Rotation,Speed,(int)LastLevel,(int)LastProfession);
		if(Database->LastError()[0] != '\0')
			DisplayError("MYSQL Error: %s",Database->LastError());

		printf("Dumped Spawn  LocalID: %u, ZoneID: %u, SpawnX: %f, SpawnY: %f, Plane: %u, Rotation: %f, Speed: %f, Level: %u, Profession: %u\n",LocalId,LastZoneID,X,Y,Plane,Rotation,Speed,(int)LastLevel,(int)LastProfession);
	}
	else
	{
		result->~MySQLQueryResult();
	}

	
	map<long,string>::iterator it;
	it = NPCNames.find(AgentId);
	if(it != NPCNames.end())
	{
		result = Database->Query("SELECT * FROM npc_spawns WHERE LocalID=%i AND LocalAppearance='%s'",LocalId,it->second.c_str());
		if(result)
		{
			result->~MySQLQueryResult();
			return;
		}

		Database->Execute("UPDATE npc_spawns SET LocalAppearance='%s' WHERE LocalID=%i AND SpawnX=%f AND SpawnY=%f AND Plane=%i",it->second.c_str(),LocalId,X,Y,Plane);
		if(Database->LastError()[0] != '\0')
			DisplayError("MYSQL Error: %s",Database->LastError());

		printf("Dumped Name: %s with LocalID: %i\n",it->second.c_str(),LocalId);
	}
}
#pragma endregion