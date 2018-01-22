using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class NodeDAL: BaseDALMySql<Node> , INodeDAL
	{
		public static NodeDAL Instance
		{
			get
			{
				return new NodeDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public NodeDAL()
		: base("Node", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}