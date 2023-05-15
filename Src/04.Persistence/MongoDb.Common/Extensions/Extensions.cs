using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Common.Extensions
{
    public static class Extensions
    {
        public static bool TryObjectId(this string input, out Nullable<ObjectId> outObj)
        {
            outObj = null;
            if (String.IsNullOrEmpty(input))
                return true;

            if (ObjectId.TryParse(input, out ObjectId converted))
            {
                outObj = converted;
                return true;
            }

            return false;
        }

        public static ObjectId ToObjectId(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return ObjectId.Empty;

            if (ObjectId.TryParse(input, out ObjectId converted))
            {
                return converted;
            }

            return ObjectId.Empty;
        }

    }
}
