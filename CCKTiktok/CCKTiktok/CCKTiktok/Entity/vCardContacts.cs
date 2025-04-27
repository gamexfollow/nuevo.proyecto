using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CCKTiktok.Entity
{
	public static class vCardContacts
	{
		public static List<Contacts> CreateList(List<string> lstPhone, List<string> lstFirstName, List<string> lstLastName, int top = 100)
		{
			List<Contacts> list = new List<Contacts>();
			foreach (string item in lstPhone)
			{
				list.Add(new Contacts
				{
					Phone = item
				});
			}
			List<Contacts> randomElements = list.GetRandomElements((list.Count > top) ? top : (list.Count / 2));
			foreach (Contacts item2 in randomElements)
			{
				int index = new Random().Next(0, lstFirstName.Count);
				int index2 = new Random().Next(0, lstLastName.Count);
				Thread.Sleep(5);
				item2.Name = lstFirstName[index].ToString() + " " + lstLastName[index2].ToString();
			}
			return randomElements;
		}

		public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
		{
			return list.OrderBy((T arg) => Guid.NewGuid()).Take(elementsCount).ToList();
		}
	}
}
