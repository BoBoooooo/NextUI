<%@ Page Language="C#" %>
<% 
    Next.WorkFlow.Utility.Tools.CheckLogin();
    string id = Request.QueryString["id"];
    if(!id.IsGuid())
    {
        Response.Write("参数错误!");
        Response.End();
    }
    Next.WorkFlow.BLL.WorkFlowFormBLL WFF = new Next.WorkFlow.BLL.WorkFlowFormBLL();
    
    var wff = WFF.Get(id.ToGuid());
    
    if(wff==null)
    {
        Response.Write("参数错误!");
        Response.End();
    }
    wff.Status = 2;
    WFF.Update(wff);

    //Next.WorkFlow.BLL.AppLibraryBLL APP = new Next.WorkFlow.BLL.AppLibraryBLL();
    //var app = APP.GetByCode(id);
    //if(app!=null)
    //{
    //    APP.Delete(app.ID);
    //}

    RoadFlow.Platform.Log.Add("删除了流程表单", wff.Serialize(), RoadFlow.Platform.Log.Types.流程相关);

    Response.Write("1");
    Response.End();
%>