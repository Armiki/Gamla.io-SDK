using System.Collections;

namespace Gamla.Logic
{
    public interface IPoolsRepository< in TKey >
    {
        #region Public Members
        /// <summary>
        /// Add pool to repository
        /// </summary>
        void AddPool< TPoolObject >( TKey key, IObjectPool< TPoolObject > pool ) where TPoolObject : class;

        /// <summary>
        /// Get pool by the key and object type
        /// </summary>
        IObjectPool< TPoolObject > GetPool< TPoolObject >( TKey key ) where TPoolObject : class;

        /// <summary>
        /// Return true if pool of IObjectPool< TPoolObject > with id exists in repo
        /// </summary>
        bool HasInPool< TPoolObject >( TKey key ) where TPoolObject : class;

        /// <summary>
        /// Remove pool with key
        /// </summary>
        /// <param name="key">pool key to remove</param>
        void RemovePool( TKey key );

        //TODO: make base class for pools
        /// <summary>
        /// Get all 
        /// </summary>
        /// <returns>Get all pools</returns>
        IEnumerable GetAll();

        /// <summary>
        /// Clear pool
        /// </summary>
        void Clear();
        #endregion
    }
}