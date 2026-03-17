using System;
using System.Collections;
using System.Collections.Generic;

namespace _Project.Develop.Runtime.Utilities.DataManagement.DataProviders
{
    public abstract class DataProvider<TData> : IDataProvider where TData : ISaveData
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly List<IDataWriter<TData>> _writers = new();
        private readonly List<IDataReader<TData>> _readers = new();
        private TData _data;

        protected DataProvider(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        public void RegisterWriter(IDataWriter<TData> writer)
        {
            if(_writers.Contains(writer))
                throw new InvalidOperationException($"The writer {writer.GetType()} is already registered");
            
            _writers.Add(writer);
        }
        
        public void RegisterReader(IDataReader<TData> reader)
        {
            if(_readers.Contains(reader))
                throw new InvalidOperationException($"The reader {reader.GetType()} is already registered");
            
            _readers.Add(reader);
        }

        public IEnumerator Load()
        {
            yield return _saveLoadService.Load<TData>(loadedData => _data = loadedData);

            SendDataToReaders();
        }
        
        public IEnumerator Save()
        {
            UpdateDataFromWriters();
            
            yield return _saveLoadService.Save(_data);
        }
        
        public IEnumerator Exists(Action<bool> onExistsResult)
        {
            yield return _saveLoadService.Exists<TData>(result => onExistsResult?.Invoke(result));
        }
        
        public void Reset()
        {
            _data = GetOriginData();

            SendDataToReaders();
        }

        protected abstract TData GetOriginData();

        private void SendDataToReaders()
        {
            foreach (IDataReader<TData> reader in _readers)
                reader.ReadFrom(_data);
        }

        private void UpdateDataFromWriters()
        {
            foreach (IDataWriter<TData> writer in _writers)
                writer.WriteTo(_data);
        }
    }
}