using System;
using System.Collections.Generic;
using PoeHUD.Framework;

namespace PoeHUD.Poe.FilesInMemory
{
    public class BaseItemTypes : FileInMemory
    {
        private readonly Dictionary<string, string> contents = new Dictionary<string, string>();

        public BaseItemTypes(Memory m, int address) : base(m, address)
        {
        }

        public string Translate(string metadata)
        {
            if (!contents.ContainsKey(metadata))
            {
                loadItemTypes();
            }
            if (!contents.ContainsKey(metadata))
            {
                Console.WriteLine("Key not found in BaseItemTypes: " + metadata);
                return metadata;
            }
            return contents[metadata];
        }

        private void loadItemTypes()
        {
            foreach (int i in RecordAddresses())
            {
                string key = M.ReadStringU(M.ReadInt(i));
                string value = M.ReadStringU(M.ReadInt(i + 16));
                if (!contents.ContainsKey(key))
                {
                    contents.Add(key, value);
                }
            }
        }
    }
}