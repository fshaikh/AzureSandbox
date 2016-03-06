using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public abstract class BlobStorageClientBase
    {
        protected StorageAccountInfo _storageAccountInfo = null;

        public BlobStorageClientBase(StorageAccountInfo accountInfo)
        {
            _storageAccountInfo = accountInfo;
        }
    }
}
