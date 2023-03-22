using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
	class ListStack<T> : IEnumerable<T>
	{
		private readonly Stack<List<T>> m_Stack = new Stack<List<T>>();

		public int StackSize => m_Stack.Count;

		public IReadOnlyCollection<List<T>> Lists() => m_Stack;

		public void PushList()
		{
			m_Stack.Push(new List<T>());
		}

		public void PushList(IEnumerable<T> ts)
		{
			m_Stack.Push(new List<T>(ts));
		}

		public List<T> PopList()
		{
			return m_Stack.Pop();
		}

		public void Add(T item)
		{
			Peek().Add(item);
		}

		public void Add(IEnumerable<T> items)
		{
			Peek().AddRange(items);
		}

		public bool Remove(T item)
		{
			return Peek().Remove(item);
		}

		public bool Contains(T item)
		{
			foreach(List<T> l in m_Stack)
			{
				if (l.Contains(item))
					return true;
			}

			return false;
		}

		public bool Contains(T item, Func<T, T, bool> comparator)
		{
			foreach(T i in this)
			{
				if (comparator(i, item))
					return true;
			}

			return false;
		}

		public bool TopContains(T item) => Peek().Contains(item);

		public List<T> Peek() => m_Stack.Peek();

		public IEnumerator<T> GetEnumerator()
		{
			foreach(List<T> l in m_Stack)
			{
				foreach (T item in l)
					yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
