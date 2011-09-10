#pragma once
#include "stdafx.h"

#define CONFIG_NAME "dumper.xml"

typedef void (*handler)(BYTE*);

struct Packet
{
	DWORD Header;
	BYTE* pData;
};

bool InitDumper();
void CloseDumper();
DWORD GetPacketLength(DWORD head,BYTE* pack);
void EnqueuePacket(Packet* pack);
void __stdcall ProcessPacketThread();

void ProcessLoadSpawnPoint(BYTE* pack);
void ProcessLoadCharName(BYTE* pack);

void ProcessNPCPacket(BYTE* pack);
void ProcessNPCGraphics(BYTE* pack);
void ProcessNPCName(BYTE* pack);
void ProcessNPCSetMiscCape(BYTE* pack);