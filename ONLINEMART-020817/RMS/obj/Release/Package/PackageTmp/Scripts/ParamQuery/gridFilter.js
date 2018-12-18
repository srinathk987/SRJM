//function filterhandler(evt, ui) {
//    debugger;
//    var $toolbar = $grid.find('.pq-toolbar-search'),
//                $value = $toolbar.find(".filterValue"),
//                value = $value.val(),
//                condition = "contain",
//                dataIndx = $toolbar.find(".filterColumn").val(),


//                filterObject = [];
//    var CM = $grid.pqGrid("getColModel");
//    for (var i = 0, len = CM.length; i < len; i++) {
//        var dataIndx = CM[i].dataIndx;
//        filterObject.push({ dataIndx: dataIndx, condition: condition, value: value });
//    }

//    $grid.pqGrid("filter", {
//        oper: 'replace',
//        data: filterObject
//    });
//}

//function filterRender(ui) {
//    var val = ui.cellData,
//                filter = ui.column.filter;
//    if (filter && filter.on && filter.value) {
//        debugger;

//        var valUpper = val.toUpperCase();
//        var txt = filter.value;
//        txt = (txt == null) ? "" : txt.toString();
//        var txtUpper = txt.toUpperCase();

//        indx = valUpper.indexOf(txtUpper);
//        if (indx >= 0) {
//            var txt1 = val.substring(0, indx);
//            var txt2 = val.substring(indx, indx + txt.length);
//            var txt3 = val.substring(indx + txt.length);
//            return txt1 + "<span style='background:red; color:#ffffff;'>" + txt2 + "</span>" + txt3;
//        }
//        else {
//            return val;
//        }
//    }
//    else {
//        return val;
//    }
//}


function filterhandler(evt, ui) {
    
    debugger;
    var $toolbar = $grid.find('.pq-toolbar-search'),
                $value = $toolbar.find(".filterValue"),
                value = $value.val(),
                condition = "contain",
                dataIndx = $toolbar.find(".filterColumn").val(),


                filterObject = [];
    var CM = $grid.pqGrid("getColModel");
    for (var i = 0, len = CM.length; i < len; i++) {
        var dataIndx = CM[i].dataIndx;
        filterObject.push({ dataIndx: dataIndx, condition: condition, value: value });
    }

    $grid.pqGrid("filter", {
        oper: 'replace',
        data: filterObject
    });
}

function filterRender(ui) {
    debugger;
    var val = ui.cellData,
                filter = ui.column.filter;
    if (filter && filter.on && filter.value) {
        debugger;
        val = '' + val;
        var valUpper = val.toUpperCase();
        var txt = filter.value;
        txt = (txt == null) ? "" : txt.toString();
        var txtUpper = txt.toUpperCase();

        indx = valUpper.indexOf(txtUpper);
        if (indx >= 0) {
            var txt1 = val.substring(0, indx);
            var txt2 = val.substring(indx, indx + txt.length);
            var txt3 = val.substring(indx + txt.length);
            return txt1 + "<span style='background:green; color:#ffffff;'>" + txt2 + "</span>" + txt3;
        }
        else {
            return val;
        }
    }
    else {
        return val;
    }
}