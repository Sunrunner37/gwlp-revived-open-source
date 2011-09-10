#include "stdafx.h"
#include "MySQLDatabase.h"
#include "GWLP Dumper.h"

BYTE* ProcessPacketStart = NULL;
BYTE* ProcessPacketEnd = NULL;

void __stdcall ProcessPacket(BYTE* pack,DWORD Header,unsigned int size)
{
	DWORD length = GetPacketLength(Header,pack + 2);
	if(length != 0 && length <= size)
	{
		Packet* p = new Packet();
		p->Header = Header;
		p->pData = new BYTE[length];
		memcpy(p->pData,pack + 2,length);
		EnqueuePacket(p);
	}
}

void _declspec(naked) ProcessPacketHook(){
	_asm{
		//Save Registers
		PUSH ECX
		PUSH ESP

		//End of stream pointer ESI+48
		MOV EAX, DWORD PTR DS:[ESI+0x44]
		MOV ECX, DWORD PTR DS:[ESI+0x48]
		SUB EAX, 2
		SUB ECX, EAX
		PUSH ECX //Length
		PUSH EDX //Header
		PUSH EAX //Packet Start
		CALL ProcessPacket

		//Restore Registers
		POP ESP
		POP ECX

		//GW Code
		MOV EAX,DWORD PTR DS:[ESI+0x18]
		MOV EDX,DWORD PTR SS:[EBP-0x4]
		MOV DWORD PTR DS:[ESI+0x858],EDX

		JMP ProcessPacketEnd
	}
}

void FindOffsets(){
	unsigned char* start = (unsigned char*)0x00401000;
	unsigned char* end = (unsigned char*)0x00800000;
	
	BYTE ProcessPacketCode[] = { 0x8B, 0x46, 0x18, 0x8B, 0x55, 0xFC, 0x89, 0x96, 0x58, 0x08, 0x00, 0x00, 0x8B, 0x08 };//{0x8D, 0x8E, 0x58, 0x08, 0x00, 0x00, 0xFF, 0x50, 0x08};

	while(start != end)
	{
		if(!memcmp(start,ProcessPacketCode,sizeof(ProcessPacketCode)))
		{
			ProcessPacketStart = start;
			ProcessPacketEnd = start + 0x0C;
			return;
		}
		start++;
	}
}

void WriteJMP(BYTE* location, BYTE* newFunction){
	DWORD dwOldProtection;
	VirtualProtect(location, 5, PAGE_EXECUTE_READWRITE, &dwOldProtection);
	location[0] = 0xE9;
	*((DWORD*)(location + 1)) = (DWORD)(newFunction - location) - 5;
	VirtualProtect(location, 5, dwOldProtection, &dwOldProtection);
}

BOOL APIENTRY DllMain( HMODULE hModule,DWORD  ul_reason_for_call,LPVOID lpReserved)
{
	
	
	DWORD dwOldProtection;
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		if(!InitDumper())
		{
			ExitProcess(-1);
		}
		FindOffsets();
		if(!ProcessPacketStart)
			DisplayError("Unable to find Process Packet Start!");
		else
		{
			VirtualProtect(ProcessPacketStart, 0x0c, PAGE_EXECUTE_READWRITE, &dwOldProtection);
			ProcessPacketStart[7] = 0x90;
			ProcessPacketStart[8] = 0x90;
			ProcessPacketStart[9] = 0x90;
			ProcessPacketStart[10] = 0x90;
			VirtualProtect(ProcessPacketStart, 0x0c, dwOldProtection, NULL);
			WriteJMP(ProcessPacketStart,(BYTE*)ProcessPacketHook);
		}
		AllocConsole();
		SetConsoleTitleA("GWLP-R Dumper by ACB");
		FILE *fh;
		freopen_s(&fh, "CONOUT$", "wb", stdout);
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		CloseDumper();
		FreeConsole();
		break;
	}
	return TRUE;
}

