using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Attachment.BLL;
using Next.Attachment.Entity;
using Next.Controllers;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Next.Framework.Core.Commons;
using Next.Commons;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.Common;
namespace Next.Areas.Admin.Controllers
{
    public class DictDataController : BusinessController<DictDataBLL, DictData>
    {
        //
        // GET: /Jssjw/Loan/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 批量添加字典数据操作
        /// </summary>
        /// <param name="DictType_ID">字典类型</param>
        /// <param name="Seq">排序开始或前缀</param>
        /// <param name="Data">批量插入的内容</param>
        /// <param name="SplitType">分开类型，分隔符分开（Split）还是行分割（Line）</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        public ActionResult BatchInsert(string DictType_ID, string Seq, string Data, string SplitType, string Remark)
        {
            bool result = false;
            if (string.IsNullOrEmpty(DictType_ID) || string.IsNullOrEmpty(Data))
            {
                return Content(result);
            }

            string[] arrayItems = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int intSeq = -1;
            int seqLength = 3;
            string strSeq = Seq;
            if (int.TryParse(strSeq, out intSeq))
            {
                seqLength = strSeq.Length;
            }

            if (arrayItems != null && arrayItems.Length > 0)
            {
                DbTransaction trans = BLLFactory<DictDataBLL>.Instance.CreateTransaction();
                if (trans != null)
                {
                    try
                    {
                        #region MyRegion
                        foreach (string strItem in arrayItems)
                        {
                            if (SplitType.Equals("split", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrWhiteSpace(strItem))
                                {
                                    string[] dataItems = strItem.Split(new char[] { ',', '，', ';', '；', '/', '、' });
                                    foreach (string dictData in dataItems)
                                    {
                                        #region 保存数据
                                        string seq = "";
                                        if (intSeq > 0)
                                        {
                                            seq = (intSeq++).ToString().PadLeft(seqLength, '0');
                                        }
                                        else
                                        {
                                            seq = string.Format("{0}{1}", strSeq, intSeq++);
                                        }

                                        InsertDictData(DictType_ID, dictData, seq, Remark, trans);
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                #region 保存数据
                                if (!string.IsNullOrWhiteSpace(strItem))
                                {
                                    string seq = "";
                                    if (intSeq > 0)
                                    {
                                        seq = (intSeq++).ToString().PadLeft(seqLength, '0');
                                    }
                                    else
                                    {
                                        seq = string.Format("{0}{1}", strSeq, intSeq++);
                                    }

                                    InsertDictData(DictType_ID, strItem, seq, Remark, trans);
                                }
                                #endregion
                            }
                        }
                        #endregion

                        trans.Commit();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        LogTextHelper.Error(ex);
                    }
                }
            }

            return Content(result);
        }

        /// <summary>
        /// 使用事务参数，插入数据，最后统一提交事务处理
        /// </summary>
        /// <param name="dictData">字典数据</param>
        /// <param name="seq">排序</param>
        /// <param name="trans">事务对象</param>
        private void InsertDictData(string dictTypeId, string dictData, string seq, string note, DbTransaction trans)
        {
            if (!string.IsNullOrWhiteSpace(dictData))
            {
                DictData info = new DictData();
                info.Editor = CurrentUser.Name;
                info.LastUpdated = DateTime.Now;
                info.DictTypeID = dictTypeId;
                info.Name = dictData.Trim();
                info.Value = dictData.Trim();
                info.Remark = note;
                info.Seq = seq;

                bool succeed = BLLFactory<DictDataBLL>.Instance.Insert(info, trans);
            }
        }


    }
}