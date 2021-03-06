﻿<%@ Page Language="C#" %>
<%@ Import   Namespace="Next.WorkFlow.Utility"   %>
<% 
    string sql = Request["sql"];
    string conn = Request["conn"];
    if (sql.IsNullOrEmpty() || !conn.IsGuid())
    {
        Response.Write("[]");
        Response.End();
    }
    var dbconn = new Next.WorkFlow.BLL.DBConnectionBLL().FindByID(conn.ToGuid());
    var dt = new Next.WorkFlow.BLL.DBConnectionBLL().GetDataTable(dbconn, sql);
    if (dt == null || dt.Rows.Count == 0)
    {
        Response.Write("[]");
        Response.End();
    }
    System.Text.StringBuilder json = new StringBuilder();
    json.Append("[");
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        json.Append("{");
        json.AppendFormat("\"id\":\"{0}\",", dr[0]);
        json.AppendFormat("\"title\":\"{0}\"", dt.Columns.Count > 1 ? dr[1] : dr[0]);
        json.Append("},");
    }
    Response.Write(json.ToString().TrimEnd(',') + "]");
    Response.End();
%>