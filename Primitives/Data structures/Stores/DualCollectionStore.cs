using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {
	public class DualCollectionStore<T>: IEnumerable{
		private readonly HashSet<T> collection = new HashSet<T>();
		private readonly List<T> enumerable = new List<T>();

		public T this[int idx]{
			get{
				return enumerable[idx];
			}
		}

		public int Count {
			get{
				return collection.Count;
			}
		}

		public bool Contains(T a){
			return collection.Contains(a);
		}

		public void Add(T b){
			if(!Contains(b)){
				collection.Add(b);
				enumerable.Add(b);
			}
		}

		public void Remove(T b){
			if(Contains(b)){
				collection.Remove(b);
				enumerable.Remove(b);
			}
		}

		public T Peek(){
			if(Count > 0){
				return enumerable[0];
			}else{
				return default(T);
			}
		}

		public T Pop(){
			if(Count > 0){
				T r = enumerable[0];
				Remove(r);
				return r;
			}else{
				return default(T);
			}
		}

		public IEnumerator GetEnumerator(){
			return enumerable.GetEnumerator();
		}
	}
}
