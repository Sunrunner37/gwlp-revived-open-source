using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.StaticConvert;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;


namespace ServerEngine.PacketManagement
{
        public class PacketManager
        {
                public PacketManager(Type[] packetStructs)
                {
                        packetInDict = new Dictionary<int, IPacket>();
                        packetOutDict = new Dictionary<int, IPacket>();

                        // basically, the next lines fill the dictionary with packet objects that can parse their data on their own

                        // in the following loop, get all packet-types and -templates, create delegates for reading/writing and create packets with them.
                        foreach (Type pType in packetStructs)
                        {
                                // get the packet type
                                var isFromClient = false;
                                var header = -1;
                                var attributes = pType.GetCustomAttributes(typeof(PacketAttributes), false);
                                if (attributes.Length == 1)
                                {
                                        isFromClient = ((PacketAttributes)attributes[0]).IsIncoming;
                                        header = ((PacketAttributes)attributes[0]).Header;
                                }

                                // get the nested packet template
                                var pTemplate = pType.GetNestedTypes().Where(type => (type.GetInterface(typeof(IPacketTemplate).Name) == typeof(IPacketTemplate))).ToList();

                                // check if the packet has a nested class that implements "IPacketTemplate"
                                if (pTemplate.Count() > 0)
                                {
                                        // create a reader or writer to/from the template within the packet
                                        var methodKind = isFromClient ? "CreateFromRawParser" : "CreateToRawParser";
                                        var delCreator = typeof(PacketManager).GetMethod(methodKind, BindingFlags.NonPublic | BindingFlags.Static).
                                                                MakeGenericMethod(new[] { pTemplate.FirstOrDefault() });

                                        // create a packet and pass the parser delegate
                                        var packet = (IPacket)Activator.CreateInstance(pType);
                                        packet.InitPacket(delCreator.Invoke(null, null));

                                        // add the packet unsing the IPacket interface if possible);)
                                        if (typeof(IPacket).IsAssignableFrom(packet.GetType()))
                                        {
                                                if (isFromClient)
                                                        packetInDict.Add(header, packet);
                                                else
                                                        packetOutDict.Add(header, packet);
                                        }
                                        else
                                        {
                                                throw new Exception(
                                                        "Packet not compatible with IPacket interface; missing reference and/or 'Handler' method?");
                                        }
                                }
                                else
                                {
                                        throw new Exception("Missing nested type 'IPacketTemplate'");
                                }
                        }
                }

                private readonly Dictionary<int, IPacket> packetInDict;
                private readonly Dictionary<int, IPacket> packetOutDict;

                private static object CreateFromRawParser<T>() where T : IPacketTemplate
                {
                        // create ref packet param
                        var packetParam = Expression.Parameter(typeof(T));

                        // create mem stream param
                        var memStreamParam = Expression.Parameter(typeof(MemoryStream));

                        // create the expressions list
                        var expressionsBlock = new List<Expression>();

                        //// skip header
                        //expressionsBlock.Add(
                        //        Expression.Call(
                        //        memStreamParam, typeof(MemoryStream).GetMethod("Seek"),
                        //        Expression.Constant((long)2),
                        //        Expression.Constant(SeekOrigin.Begin)));

                        // iterate fields
                        FieldInfo lastFieldInfo = null;
                        foreach (var fieldInfo in typeof(T).GetFields())
                        {
                                if (!fieldInfo.IsLiteral)
                                {
                                        var fieldExp = Expression.Field(packetParam, fieldInfo);

                                        if (fieldInfo.FieldType == typeof(byte))
                                        {
                                                expressionsBlock.Add(
                                                        Expression.Call(typeof(RawConverter).GetMethod("ReadByte"),
                                                                        fieldExp, memStreamParam));
                                        }
                                        else if (fieldInfo.FieldType == typeof(UInt16))
                                        {
                                                expressionsBlock.Add(
                                                        Expression.Call(typeof(RawConverter).GetMethod("ReadUInt16"),
                                                                        fieldExp, memStreamParam));
                                        }
                                        else if (fieldInfo.FieldType == typeof(UInt32))
                                        {
                                                expressionsBlock.Add(
                                                        Expression.Call(typeof(RawConverter).GetMethod("ReadUInt32"),
                                                                        fieldExp, memStreamParam));
                                        }
                                        else if (fieldInfo.FieldType == typeof(float))
                                        {
                                                expressionsBlock.Add(
                                                        Expression.Call(typeof(RawConverter).GetMethod("ReadFloat"),
                                                                        fieldExp, memStreamParam));
                                        }
                                        else if (fieldInfo.FieldType == typeof(string))
                                        {
                                                expressionsBlock.Add(
                                                        Expression.Call(typeof(RawConverter).GetMethod("ReadUTF16"),
                                                                        fieldExp, memStreamParam));
                                        }
                                        else if (fieldInfo.FieldType == typeof(byte[]))
                                        {
                                                var attributes =
                                                        fieldInfo.GetCustomAttributes(
                                                                typeof(PacketFieldType), false);
                                                if (attributes.Length == 1)
                                                {
                                                        if (((PacketFieldType)attributes[0]).ConstSize)
                                                        {
                                                                // in case thats a const size array:
                                                                var size = (UInt16)((PacketFieldType)attributes[0]).MaxSize;
                                                                expressionsBlock.Add(
                                                                        Expression.Call(
                                                                                typeof(RawConverter).GetMethod(
                                                                                        "ReadByteAr"), fieldExp,
                                                                                memStreamParam,
                                                                                Expression.Convert(Expression.Constant(size), typeof(int))));
                                                        }
                                                        else if (lastFieldInfo != null)
                                                        {
                                                                // in case that array has a length prefix:
                                                                expressionsBlock.Add(
                                                                        Expression.Call(
                                                                                typeof(RawConverter).GetMethod(
                                                                                        "ReadByteAr"), fieldExp,
                                                                                memStreamParam,
                                                                                Expression.Convert(Expression.Field(packetParam,
                                                                                                 lastFieldInfo), typeof(int))));
                                                        }
                                                        else
                                                        {
                                                                throw new Exception(
                                                                        "Variable array in packet Struct, but no length prefix struct field found!");
                                                        }
                                                }

                                        }
                                        else if (fieldInfo.FieldType == typeof(UInt16[]))
                                        {

                                                var attributes =
                                                        fieldInfo.GetCustomAttributes(
                                                                typeof(PacketFieldType), false);
                                                if (attributes.Length == 1)
                                                {
                                                        if (((PacketFieldType)attributes[0]).ConstSize)
                                                        {
                                                                // in case thats a const size array:
                                                                var size = (UInt16)((PacketFieldType)attributes[0]).MaxSize;
                                                                expressionsBlock.Add(
                                                                        Expression.Call(
                                                                                typeof(RawConverter).GetMethod(
                                                                                        "ReadUInt16Ar"), fieldExp,
                                                                                memStreamParam,
                                                                                Expression.Convert(Expression.Constant(size), typeof(int))));
                                                        }
                                                        else if (lastFieldInfo != null)
                                                        {
                                                                // in case that array has a length prefix:
                                                                expressionsBlock.Add(
                                                                        Expression.Call(
                                                                                typeof(RawConverter).GetMethod("ReadUInt16Ar"), fieldExp, memStreamParam,
                                                                                Expression.Multiply(Expression.Convert(Expression.Field(packetParam,lastFieldInfo), typeof(int)),
                                                                                Expression.Constant(2))));
                                                        }
                                                        else
                                                        {
                                                                throw new Exception(
                                                                        "Variable array in packet Struct, but no length prefix struct field found!");
                                                        }
                                                }
                                        }
                                        else if (fieldInfo.FieldType == typeof(UInt32[]))
                                        {
                                                var attributes =
                                                        fieldInfo.GetCustomAttributes(
                                                                typeof(PacketFieldType), false);
                                                if (attributes.Length == 1)
                                                {
                                                        if (((PacketFieldType)attributes[0]).ConstSize)
                                                        {
                                                                // in case thats a const size array:
                                                                var size = (UInt16)((PacketFieldType)attributes[0]).MaxSize;
                                                                expressionsBlock.Add(
                                                                        Expression.Call(
                                                                                typeof(RawConverter).GetMethod(
                                                                                        "ReadUInt32Ar"), fieldExp,
                                                                                memStreamParam,
                                                                                Expression.Convert(Expression.Constant(size), typeof(int))));
                                                        }
                                                        else if (lastFieldInfo != null)
                                                        {
                                                                // in case that array has a length prefix:
                                                                expressionsBlock.Add(
                                                                        Expression.Call(
                                                                                typeof(RawConverter).GetMethod("ReadUInt32Ar"), fieldExp, memStreamParam,
                                                                                Expression.Multiply(Expression.Convert(Expression.Field(packetParam,lastFieldInfo), typeof(int)),
                                                                                Expression.Constant(4))));
                                                        }
                                                        else
                                                        {
                                                                throw new Exception(
                                                                        "Variable array in packet Struct, but no length prefix struct field found!");
                                                        }
                                                }
                                        }

                                        // set lastFieldInfo
                                        lastFieldInfo = fieldInfo;
                                }
                        }

                        if (expressionsBlock.Count != 0)
                        {
                                return Expression.Lambda(typeof(PacketParser<>).MakeGenericType(new[] { typeof(T) }),
                                                         Expression.Block(expressionsBlock),
                                                         new[] { packetParam, memStreamParam }).Compile();
                        }
                        return null;
                }

                private static object CreateToRawParser<T>() where T : IPacketTemplate
                {
                        // create ref packet param
                        var packetParam = Expression.Parameter(typeof(T));

                        // create mem stream param
                        var memStreamParam = Expression.Parameter(typeof(MemoryStream));

                        // create the expressions list
                        var expressionsBlock = new List<Expression>();

                        // add the header
                        var headerExp = Expression.Property(packetParam, typeof (T).GetProperty("Header"));
                        expressionsBlock.Add(Expression.Call(typeof(RawConverter).GetMethod("WriteUInt16"),
                                                                headerExp, memStreamParam));

                        // iterate fields)
                        foreach (var fieldInfo in typeof(T).GetFields())
                        {
                                var fieldExp = Expression.Field(packetParam, fieldInfo);

                                if (fieldInfo.FieldType == typeof(byte))
                                {
                                        expressionsBlock.Add(
                                                Expression.Call(typeof(RawConverter).GetMethod("WriteByte"),
                                                                fieldExp, memStreamParam));
                                }
                                else if (fieldInfo.FieldType == typeof(UInt16))
                                {
                                        expressionsBlock.Add(
                                                Expression.Call(typeof(RawConverter).GetMethod("WriteUInt16"),
                                                                fieldExp, memStreamParam));
                                }
                                else if (fieldInfo.FieldType == typeof(UInt32))
                                {
                                        expressionsBlock.Add(
                                                Expression.Call(typeof(RawConverter).GetMethod("WriteUInt32"),
                                                                fieldExp, memStreamParam));
                                }
                                else if (fieldInfo.FieldType == typeof(float))
                                {
                                        expressionsBlock.Add(
                                                Expression.Call(typeof(RawConverter).GetMethod("WriteFloat"),
                                                                fieldExp, memStreamParam));
                                }
                                else if (fieldInfo.FieldType == typeof(string))
                                {
                                        expressionsBlock.Add(
                                                Expression.Call(typeof(RawConverter).GetMethod("WriteUTF16"),
                                                                fieldExp, memStreamParam));
                                }
                                else if (fieldInfo.FieldType == typeof(byte[]))
                                {
                                        expressionsBlock.Add(
                                                Expression.Call(typeof(RawConverter).GetMethod("WriteByteAr"),
                                                                fieldExp, memStreamParam));
                                }
                                else if (fieldInfo.FieldType == typeof(UInt16[]))
                                {
                                        expressionsBlock.Add(
                                                Expression.Call(typeof(RawConverter).GetMethod("WriteUInt16Ar"),
                                                                fieldExp, memStreamParam));
                                }
                                else if (fieldInfo.FieldType == typeof(UInt32[]))
                                {
                                        expressionsBlock.Add(
                                                Expression.Call(typeof(RawConverter).GetMethod("WriteUInt32Ar"),
                                                                fieldExp, memStreamParam));
                                }
                        }

                        if (expressionsBlock.Count != 0)
                        {
                                return Expression.Lambda(typeof(PacketParser<>).MakeGenericType(new[] { typeof(T) }),
                                                         Expression.Block(expressionsBlock),
                                                         new[] { packetParam, memStreamParam }).Compile();
                        }
                        return null;
                }

                public void ProcessPackets()
                {
                        // Determines how many tasks will be created as a maximum.
                        var msgCount = QueuingService.NetInQueue.Count;
                        var taskCount = (msgCount > 10) ? 10 : msgCount;

                        Parallel.For(0, taskCount, delegate(int i)
                        {
                                NetworkMessage tmpMessage;
                                QueuingService.NetInQueue.TryDequeue(out tmpMessage);

                                while (tmpMessage.PacketData.Position < tmpMessage.PacketData.Length)
                                {
                                        var buffer = new byte[2];
                                        tmpMessage.PacketData.Read(buffer, 0, 2);
                                        var header = BitConverter.ToUInt16(buffer, 0);

                                        IPacket pck;
                                        packetInDict.TryGetValue(header, out pck);


                                        if (pck == null || !pck.Handler(ref tmpMessage))
                                        {
                                                throw new Exception(
                                                        string.Format(
                                                                "Was unable to handle packet [{0}]. Missing packet handler?",
                                                                header));
                                        }
                                }
                        });

                        // Determines how many tasks will be created as a maximum.
                        msgCount = QueuingService.PostProcessingQueue.Count;
                        taskCount = (msgCount > 10) ? 10 : msgCount;

                        //Parallel.For(0, taskCount, delegate(int i)
                        for (int i = 0; i < taskCount; i++)
                        {
                                NetworkMessage tmpMessage;
                                QueuingService.PostProcessingQueue.TryDequeue(out tmpMessage);

                                IPacket pck;
                                packetOutDict.TryGetValue(tmpMessage.Header, out pck);

                                if (pck == null || !pck.Handler(ref tmpMessage))
                                {
                                        throw new Exception(
                                                string.Format("Was unable to handle packet [{0}]. Missing packet handler?", tmpMessage.Header));
                                }
                        }
                }
        }
}
