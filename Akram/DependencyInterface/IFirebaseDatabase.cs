using Akram.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Akram.DependencyInterface
{
    public interface IFirebaseDatabase
    {
        void MakeConnection();

        void MakeConnectionForReferesh();

        void CheckGiftTaken();

        void GetGiftsMyCollection();

        void CheckGiftForScan();

        void Collect(CollectModel collectModel);

        void DeleteCollection(string id, string itemId,string userId);

        void GetLocations();

        void CheckGiftExists();

        void GetScennedCollectionData();

        void GiftDetailsCheckTaken();
    }
}
