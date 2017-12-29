/**
 * Title: 公用JS
 * Description: 公用JS
 * Copyright: Copyright (c) 2011
 * Company: 易象
 * @author 钱嘉
 * @version 1.0 2011-11-22
 */

/**
 * 初始化Easyui DataGrid属性
 */

$.extend($.fn.linkbutton.methods, {
    /**
     * 激活选项（覆盖重写）
     * @param {Object} jq
     */
    enable: function (jq) {
        return jq.each(function () {
            var state = $.data(this, 'linkbutton');
            if ($(this).hasClass('l-btn-disabled')) {
                var itemData = state._eventsStore;
                //恢复超链接
                if (itemData.href) {
                    $(this).attr("href", itemData.href);
                }
                //回复点击事件
                if (itemData.onclicks) {
                    for (var j = 0; j < itemData.onclicks.length; j++) {
                        $(this).bind('click', itemData.onclicks[j]);
                    }
                }
                //设置target为null，清空存储的事件处理程序
                itemData.target = null;
                itemData.onclicks = [];
                $(this).removeClass('l-btn-disabled');
            }
        });
    },
    /**
     * 禁用选项（覆盖重写）
     * @param {Object} jq
     */
    disable: function (jq) {
        return jq.each(function () {
            var state = $.data(this, 'linkbutton');
            if (!state._eventsStore)
                state._eventsStore = {};
            if (!$(this).hasClass('l-btn-disabled')) {
                var eventsStore = {};
                eventsStore.target = this;
                eventsStore.onclicks = [];
                //处理超链接
                var strHref = $(this).attr("href");
                if (strHref) {
                    eventsStore.href = strHref;
                    $(this).attr("href", "javascript:void(0)");
                }
                //处理直接耦合绑定到onclick属性上的事件
                var onclickStr = $(this).attr("onclick");
                if (onclickStr && onclickStr != "") {
                    eventsStore.onclicks[eventsStore.onclicks.length] = new Function(onclickStr);
                    $(this).attr("onclick", "");
                }
                //处理使用jquery绑定的事件
                var eventDatas = $(this).data("events") || $._data(this, 'events');
                if (eventDatas["click"]) {
                    var eventData = eventDatas["click"];
                    for (var i = 0; i < eventData.length; i++) {
                        if (eventData[i].namespace != "menu") {
                            eventsStore.onclicks[eventsStore.onclicks.length] = eventData[i]["handler"];
                            $(this).unbind('click', eventData[i]["handler"]);
                            i--;
                        }
                    }
                }
                state._eventsStore = eventsStore;
                $(this).addClass('l-btn-disabled');
            }
        });
    }
});
$(document).ready(function(){
	if ($.fn.datagrid){
		$.fn.datagrid.defaults.width = 1024;			//宽
		$.fn.datagrid.defaults.height = 1024;			//长
		$.fn.datagrid.defaults.pageList = [30,50,100,500,1000];	//分页数列表
		$.fn.datagrid.defaults.pagination = true;		//是否在Grid底部显示分页工具栏
		$.fn.datagrid.defaults.sortOrder = 'desc';		//默认排序规则
		$.fn.datagrid.defaults.collapsible = true;		//Grid是否可收缩
		$.fn.datagrid.defaults.rownumbers = true;		//Grid显示行数
		$.fn.datagrid.defaults.striped = true;			//交替显示行背景
		$.fn.datagrid.defaults.remoteSort = true;		//通过远程服务器对数据排序
		//$.fn.datagrid.defaults.fit = true;				//自适应
		$.fn.datagrid.defaults.fitColumns = true;		//自适应宽度
		$.fn.datagrid.defaults.rowStyler = function (index, row) {//行样式
			return 'line-height:23px;';
		};
		$.fn.datagrid.defaults.onBeforeLoad = function (param) {//清除选中项
			$(this).datagrid('clearSelections');
			return true;
		};
	}

	if($.fn.datebox){
		$.fn.datebox.defaults.editable = false;
	}

});

/**
 * 打开mywindow的窗口
 * @param title 弹出窗的标题
 * @param url Action请求路径
 * @param width 弹出窗宽度
 * @param heigth 弹出窗高度
 * @return
 */
function openMyWindow(title,url,width,heigth){
	openSpecifiedWindow("mywindow",title,url,width,heigth);
}

/**
 * 关闭mywindow打开的窗口
 * @return
 */
function closeMyWindow() {
	$('#mywindow').window('close');
}

/**
 * 打开指定的窗口
 * @param window 弹出窗所渲染的对象ID
 * @param title 弹出窗的标题
 * @param url Action请求路径
 * @param width 弹出窗宽度
 * @param heigth 弹出窗高度
 * @return
 */
function openSpecifiedWindow(window,title,url,width,heigth){
	var s_h = document.body.clientHeight;// 得到屏幕可用区域高度
	var s_i = document.body.clientWidth; // 得到屏幕可用区域宽度
	var d_h = 1;						 // 弹出传弹出后屏幕剩余的高度
	var d_i = 1;						 // 弹出传弹出后屏幕剩余的宽度

	url = encodeURI(url,"utf-8");
	if (url.indexOf("?")>-1) {
		url += "&fresh=" + Math.random();
	} else {
		url += "?fresh=" + Math.random();
	}

	//有自定义宽度且小于屏幕可用宽度时：弹出窗宽度等于自定义宽度，否则等于屏幕可用宽度-50
	width = ( width != null && width != "" && width < s_i ) ? width :  ( s_i - d_i );
	//有自定义高度且小于屏幕可用高度时：弹出窗高度等于自定义高度，否则等于屏幕可用高度-50
	heigth = (heigth != null && heigth != "" && heigth < s_h ) ? heigth : ( s_h - d_h );
	$('#'+window).window({
		title: title,
		left: (s_i - width)/2,
		top: (s_h - heigth)/2,
		width: width,
		height: heigth,
		modal: false,
		cache:false,
		shadow: true,
		minimizable:false,
		maximizable:true,
		closable:true,
		collapsible:true,
		closed: false,
		draggable:true,
		resizable:true,
		href: url,
		onOpen:function(){
			$('select').addClass("tntmng_undisplay");
		},
		onClose:function(){
			$('select').removeClass("tntmng_undisplay");
		}
	});
}

/**
 * 关闭指定的窗口
 * @param window 弹出窗所渲染的对象ID
 * @return
 */
function closeSpecifiedWindow(window){
	$('#'+window).window('close');
}

/**
 * 新增或修改表单
 * @param frm 表单ID
 * @param url Action映射
 * @param fun 保存后执行的动作
 * @return
 */
function saveOrUploadForm(frm,url,fun_name){
	$('#'+frm).form('submit', {
        url: url,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success:function(data){
            if (fun_name != null) {
            	var fun = new getFunByName(fun_name);
            	fun.func();
        	}
        }
	});
}


/**
 * 新增或修改表单
 * @param {Object} frm表单ID
 * @param {Object} url提交URL
 * @param {Object} prefun_name 提交前执行方法
 * @param {Object} aftfun_name 提交后执行方法
 */
function saveOrUploadForm2(frm,url,prefun_name,aftfun_name){
	var fun1 = new getFunByName(prefun_name);
    var flag=fun1.func();
	if(flag)
	{
		$('#'+frm).form('submit', {
	        url: url,
	        onSubmit: function(){
	            return $(this).form('validate');
	        },
	        success:function(data){
	        	var fun = new getFunByName(aftfun_name);
	        	fun.func();
	        }
		});
	}
}


/**
 * 新增或修改表单
 * @param {Object} frm表单ID
 * @param {Object} url提交URL
 * @param {Object} prefun_name 提交前执行方法
 * @param {Object} aftfun_name 提交后回调方法
 */
function saveOrUploadForm3(frm,url,prefun_name,aftfun_name){
	var flag=false;
	if(prefun_name)
	{
		var fun1 = new getFunByName(prefun_name);
    		flag=fun1.func();
	} else {
		flag=true;
	}

	if(flag) {
		$('#'+frm).form('submit', {
        url: url,
        onSubmit: function(){
            return $(this).form('validate');
        },
        success:function(data){
        	if(aftfun_name)
        	{
        		var fun = new getFunByName(aftfun_name);
        			fun.func(data);
        	}else
        	{
        		var fun = new getFunByName('closeMyWindow');
        			fun.func();
        	}
        }
		});
	}
}

/**
 * 根据方法名查找到方法名代表的方法
 * @param fun
 * @return
 */
function getFunByName(fun) {
	// 初始化this.func属性,
	this.func = function() { };
	try {
		//用eval方法把方法名所代表的方法当作一个对象来赋值给getFunByName的func属性。
		//如果找不到methodName这个对应的对象,则eval方法会抛异常
		this.func = eval(fun);
	} catch (e) {
		alert("找不到" + fun + "()这个方法");
	}
}

/**
 * 获取grid单参数 并执行方法 可以带参数
 * @param {Object} id --field
 * @param {Object} grid---gridid
 * @param {Object} fun_name--方法名
 * @param {Object} fun_param--方法参数
 */
function doMethodBySingle(id,grid,fun_name)
{
	var id = getSingleSelection(id, grid);
	if(id)
	{
		var fun = new getFunByName(fun_name);
        fun.func(id);
	}
}
/**
 * 获取grid多行参数 并执行方法 可以带参数
 * @param {Object} id --field
 * @param {Object} grid---gridid
 * @param {Object} fun_name--方法名
 * @param {Object} fun_param--方法参数
 */
function doMethodByMultiple(id,grid,fun_name)
{
	var id = getMultipleSelections(id, grid);
	if(id)
	{
		var fun = new getFunByName(fun_name);
        	fun.func(id);
	}
}

/**
 * 验证Grid是否选中且只选中单条,并返回选中记录的主键值
 * 
 * @param id 主键名称
 * @param grid Grid渲染的元素ID
 * @return
 */
function getSingleSelection(id, grid) {
	var ids = [];
	var rows = $('#' + grid).datagrid('getSelections');
	if (rows.length > 1) {
		$.messager.alert('提示信息', '选中项过多,请选其中一项！', 'info');
		return false;
	}
	if (rows.length == 0) {
		$.messager.alert('提示信息', '请先选中一项！', 'info');
		return false;
	}
	if (rows.length == 1) {
		var id = rows[0][id];
		return id;
	}
}

/**
 * 获取Grid选中项的指定列的值
 * 
 * @param id 主键名称
 * @param grid Grid渲染的元素ID
 * @return
 */
function getMultipleSelections(id, grid) {
	var ids = [];
	var rows = $('#' + grid).datagrid('getSelections');
	if (rows.length == 0) {
		if(!arguments[2])//如果设置第三个参数为true则 为空时 不提示
		$.messager.alert('提示信息', '请先选中一项！', 'info');
		return false;
	}
	var row_length = rows.length;
	for(var i=0;i<row_length;i+=1){
		ids.push(rows[i][id]);
	}
	var id = ids.join(',');
	return id;
}

/**
 * Grid中删除单条选中项
 * @param id 选中项的主键值拼接字符串
 * @param id_name 主键的字段名
 * @param url Action请求地址
 * @param fun 删除成功后执行的方法
 * @return
 */
function removeObjInGridSingle(id, id_name, url, fun) {
	$.messager.confirm('是否删除', '是否删除选中项?', function(r) {
		if (r) {
			removeObjInGrid(id, id_name, url, fun);
		}
	});
}

/**
 * Grid中批量删除选中项
 * @param id_name 主键的字段名
 * @param grid 操作的Grid所渲染的元素ID
 * @param url Action请求地址
 * @param fun 删除成功后执行的方法
 * @return
 */
function removeObjInGridBatch(id_name, grid, url, fun){
	var ids = [];
	var rows = $('#'+grid).datagrid('getSelections');
	if(rows.length == 0){
		$.messager.alert('提示信息','请先选择至少一个删除对象！','info');
		return;
	}
	$.messager.confirm('是否删除', '是否删除选中项?', function(r) {
		if (r){
			var row_length = rows.length;
			for(var i=0;i<row_length;i+=1){
				ids.push(rows[0][id_name]);
				var rowindex = $('#'+grid).datagrid('getRowIndex',rows[0][id_name]);
				$('#'+grid).datagrid('unselectRow',rowindex);
			}
			var id = ids.join(',');
			removeObjInGrid(id, id_name, url, fun);
		}
	});
}

/**
 * Jquery Ajax 删除Grid中选中项
 * @param id 选中项的主键值拼接字符串
 * @param id_name 主键的字段名
 * @param url Action请求地址
 * @param fun 删除成功后回调函数
 * @return
 */
function removeObjInGrid(id, id_name, url, fun_name) {
	$.ajax( {
		type : "POST",
		url : url,
		data : id_name + '=' + id,
		complete : function(msg) {
	    	var fun = new getFunByName(fun_name);
	    	fun.func();
		}
	});
}

/**
 * Jquery Ajax 删除Grid中选中项
 * @param id 选中项的主键值拼接字符串
 * @param id_name 主键的字段名
 * @param url Action请求地址
 * @param fun 删除成功后回调函数
 * @return
 */
function removeObjWithConfirm(id, id_name, url, fun_name){
	$.messager.confirm('是否删除', '是否删除选中项?', function(r) {
		if (r){
			$.ajax( {
				type : "POST",
				url : url,
				data : id_name + '=' + id,
				success : function(msg) {
			    	var fun = new getFunByName(fun_name);
			    	fun.func();
				}
			});
		}
	});
}
/**
 * checkbox 全选/全不选
 * @param obj
 * @param cName
 * @return
 */
function check_all(obj, cName) {
	var checkboxs = document.getElementsByName(cName);
	for ( var i = 0; i < checkboxs.length; i+=1) {
		checkboxs[i].checked = obj.checked;
	}
}

/**
 * checkbox反选
 * @param cName
 * @return
 */
function check_inverse(cName){
	var checkboxs = document.getElementsByName(cName);
	for ( var i = 0; i < checkboxs.length; i+=1) {
		checkboxs[i].checked = !checkboxs[i].checked;
	}
}

/**
 * 得到checkbox的选中值
 * @param cName
 * @return
 */
function checkbox_val(cName){
	var arr = [];
	var checkboxs = document.getElementsByName(cName);
	for ( var i = 0; i < checkboxs.length; i+=1) {
		var val = checkboxs[i].value;
		if(checkboxs[i].checked)
			arr.push(val);
	}
	var sel_val = arr.join(",");
	return sel_val;
}

/**
 * 获取树选中节点和父节点
 * @param tree_name 树ID
 * @return
 */
function getTreeSelectNode(tree_name){
	var nodes = $('#'+tree_name).tree('getChecked');
	var arr = [];
	var str = '';
	var v_str = '|';
	for(var i=0; i<nodes.length; i+=1){
		if(nodes[i].id != '0')
		{
			arr.push(nodes[i].id);
			if(nodes[i].attributes.pid != '0') arr = getParentNodeID(arr,nodes[i],tree_name);
		}
	}
	for(var i=0; i<arr.length; i+=1){
		if(v_str.indexOf("|"+arr[i]+"|") == -1){
			v_str += arr[i]+"|";
			if (str != '') str += ',';
			str += arr[i];	
		}
	}
	return str;
}

/**
 * 获取树中父节点
 *   arr 拼接的字符串
 * @param node 节点
 * @param tree_name 树ID
 * @return
 */
function getParentNodeID(arr,node,tree_name){
	var p_node = $('#'+tree_name).tree('getParent', node.target);
	var p_id = p_node.id;
	arr.push(p_id);
	if(p_node.attributes.pid != '0') str = getParentNodeID(arr,p_node,tree_name);
	return arr;
}

/**
 *
 *  @param url 请求地址
 *  @param fun_name 保存后执行的动作
 */
function optWithConfirm(url,fun_name,info_title, info){
	if(info_title == null || info_title == ""){
		info_title = "操作";
	}
	if(info == null || info == ""){
		info = "您确定要执行此操作吗?";
	}
	$.messager.confirm(info_title, info, function(r) {
		if (r){
		  $.ajax({
		      url:url,
			  type:"POST",
              datatype:"json",
              complete : function(msg) {
			    	var fun = new getFunByName(fun_name);
			    	fun.func();
				}
		});
	 }
	});
}

/**
 * Jquery对象转JSON对象
 * @param obj
 * @return
 */
function objToJSON(obj){
	var str = JSON.stringify(obj);
	return str;
}

/**
 * 表单serialize时中文乱码的问题
 * @param objs
 * @return
 */
function serialize(objs) {
    var parmString = $(objs).serialize();   
    var parmArray = parmString.split("&");
    var parmStringNew="";
    $.each(parmArray,function(index,data){
        var li_pos = data.indexOf("=");
        if(li_pos >0){
            var name = data.substring(0,li_pos);
            var value = escape(decodeURIComponent(data.substr(li_pos+1)));
            var parm = name+"="+value;
            parmStringNew = parmStringNew=="" ? parm : parmStringNew + '&' + parm;
        }
    });
    return parmStringNew;
};