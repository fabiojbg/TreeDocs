using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.MongoDb.Extensions
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
    }
}
