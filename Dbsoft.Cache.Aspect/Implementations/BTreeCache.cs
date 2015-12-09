namespace CacheAspect.Implementations
{
	using System;
	using BplusDotNet;
	using System.IO;

	class BTreeCache : ICache
    {
        private static SerializedTree _treeCache;
        private static string _datafile;
        private static string _treefile;

        public BTreeCache()
        {
            _datafile = CacheService.DiskPath + "datafile";
            _treefile = CacheService.DiskPath + "treefile";
            LoadCache();
        }

        ~BTreeCache()
        {
            CloseCache();
        }

        public void LoadCache()
        {
            if (_treeCache == null)
            {
                if (File.Exists(_treefile) && File.Exists(_datafile))
                {
                    _treeCache = new SerializedTree(hBplusTreeBytes.ReOpen(_treefile, _datafile));
                }
                else
                {
                    _treeCache = new SerializedTree(hBplusTreeBytes.Initialize(_treefile, _datafile, 500));
                }
                _treeCache.SetFootPrintLimit(10);
            }
        }

        public void SaveCache()
        {
            if (_treeCache != null)
            {
                _treeCache.Commit();
            }
        }

        public void CloseCache()
        {
            _treeCache.Shutdown();
        }

        public object this[string key]
        {
            get
            {
                if (_treeCache.ContainsKey(key))
                {
                    return _treeCache[key];
                }
                return null;
            }
            set
            {
                _treeCache[key] = value;
                SaveCache();
            }
        }

     
        public bool Contains(string key)
        {
            return _treeCache.ContainsKey(key);
        }

        public void Delete(string key)
        {
            _treeCache.RemoveKey(key);
            SaveCache();
        }

		public void DeleteAll(string @group)
		{
			throw new NotImplementedException();
		}

		public void DeleteAll()
	    {
		    throw new NotImplementedException();
	    }
    }
}
