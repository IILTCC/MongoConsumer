﻿using MongoConsumerLibary.MongoConnection.Enums;
using System.Collections.Generic;

namespace MongoConsumerLibary.MongoConnection.Collections
{
    public class BaseBoxCollection : ExpirableCollection
    {
        public string IcdType { get; set; }
        public string CompressedData { get; set; }
   }
}
