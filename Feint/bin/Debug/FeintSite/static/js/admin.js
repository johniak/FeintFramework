/**
 * Created with JetBrains WebStorm.
 * User: Johniak
 * Date: 27.09.13
 * Time: 17:42
 * To change this template use File | Settings | File Templates.
 */
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define([ 'underscore' ], factory);
    } else {
        root.NestedAjaxDataSource = factory();
    }
}(this, function () {

    var NestedAjaxDataSource = function (options) {
        this._formatter = options.formatter;
        this._columns = options.columns;
        this._delay = options.delay || 0;
        this._pageIndex = options.pageIndex || 0;
        this._pageSize = options.pageSize || 10;
        this._data = options.data;
        //AJAXification by @btaylordesign
        this._data_url = options.data_url;
        this._data_key = options.data_key;
        this._initial_load_from_server = options.initial_load_from_server || false;
        this._always_load_from_server = options.always_load_from_server || false;
        this.reload_data_from_server = false;
    };

    NestedAjaxDataSource.prototype = {

        columns: function () {
            return this._columns;
        },

        data: function (options, callback) {
            var self = this;

            setTimeout(function () {
                var pageIndex = options.pageIndex || 0;
                var pageSize = options.pageSize || 10;
                var startIndex = pageIndex* pageSize;
                var url=self._data_url+startIndex+"/"+pageSize+"/";
                var count=0;
                if(options.sortProperty){
                    url+=options.sortProperty+"/"+(options.sortDirection=="asc")+"/";
                }else{
                    url+="Id/true/";
                }
                if(options.search){
                    url+=options.search+"/";
                }
                $.ajax(url, {
                    dataType: 'json',
                    async: false,
                    type: 'GET'
                }).done(function (json) {
                        data = self._data_key ? json[self._data_key] : json;
                    });
                $.ajax(self._data_url+"count/", {
                    dataType: 'json',
                    async: false,
                    type: 'GET'
                }).done(function (json) {
                        count = json;
                    });

                self.reload_data_from_server = false;
                var endIndex = startIndex + pageSize;
                var end = (endIndex > count) ? count : endIndex;
                var pages = Math.ceil(count / pageSize);
                var page = pageIndex + 1;
                var start = startIndex + 1;
                self._formatter(data);
                callback({
                    data: data,
                    start: start,
                    end: end,
                    count: count,
                    pages: pages,
                    page: page
                });

            }, this._delay)
        }
    };

    return NestedAjaxDataSource;
}));

function deleteRow(){
    $.ajax('/admin/model/'+model+'/'+id+'/', {
        dataType: 'json',
        async: false,
        type: 'DELETE'
    }).done(function (json) {
            $('#MyGrid').datagrid('reload');
            $('#delete-modal').modal('hide');
        });
}

function addRow(){
    $.ajax('/admin/model/'+model+'/', {
        dataType: 'json',
        async: false,
        type: 'POST',
        data: $('#add-form').serialize()
    }).done(function (json) {
            $('#MyGrid').datagrid('reload');
            $('#add-modal').modal('hide');
        });
}
function editRow(){
    $.ajax('/admin/model/'+model+'/'+id+'/', {
        dataType: 'json',
        async: false,
        type: 'PUT',
        data: $('#edit-form').serialize()
    }).done(function (json) {
            $('#MyGrid').datagrid('reload');
            $('#edit-modal').modal('hide');
        });
}