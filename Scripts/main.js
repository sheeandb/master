
function Submit(i) {
    var err = false;
    var x;
    if ($('#lblIntWhole').text() == "") {
        $('#lblValInterest').show();
        err = true;
    }

    if (Validate($('#txtLoanAmount').val())) {
        $('#lblValLoanAmt').show();
        err = true;
    }
    else {
        x = AlphaStrip($('#txtLoanAmount').val());
        $('#hdnLoanAmount').val(x);
    }

    if (Validate($('#txtTermOfLoan').val())) {
        $('#lblValTerm').show();
        err = true;
    }
    else {
        x = AlphaStrip($('#txtTermOfLoan').val());
        $('#hdnTermOfLoan').val(x);
    }

    if (Validate($('#txtTaxAndIns').val())) {
        $('#lblValTaxIns').show();
        err = true;
    }
    else {
        x = AlphaStrip($('#txtTaxAndIns').val());
        $('#hdnTaxAndIns').val(x);
    }

    if ($('#lblStartMonth').text() == "") {
        $('#lblValStartMonth').show();
        err = true;
    }

    if ($('#lblStartYear').text() == "") {
        $('#lblValStartYear').show();
        err = true;
    }

    if ($('#datepicker').val() == "") {
        $('#lblValCurrentDate').show();
        err = true;
    }
    else {
        $('#lblValCurrentDate').hide();
    }

    var sdate = new Date($('#lblStartMonth').text() + '/1/' + $('#lblStartYear').text()).valueOf();
    var cdate = new Date($('#datepicker').val()).valueOf();
    if (cdate < sdate) {
        $('#lblValStartDate').show();
        err = true;
    }
    else {
        $('#lblValStartDate').hide();
    }

    if (!err) {
        if (i > -1) {
            var v = $("#txtAddToPrinciple-" + i).val()
        }
        var q = i.toString();
        var sm = $('#hdnStartMonth').val();
        var sy = $('#lblStartYear').text();
        var c = parseInt($('#lblIntWhole').text()) + $('#hdnIntFrac').val();
        var L = $('#hdnLoanAmount').val();
        var n = $('#hdnTermOfLoan').val();
        var t = $('#hdnTaxAndIns').val();
        var d = $('#datepicker').val().toString();
        var qs = "?sm=" + sm + "&sy=" + sy + "&c=" + c + "&L=" + L + "&n=" + n + "&t=" + t + "&d=" + d + "&q=" + q + "&v=" + v;
        var data = { q: q, sm: sm, sy: sy, c: c, L: L, n: n, t: t, d: d, q: q }
        $('#divDataTableContainer').empty()
        $("#progressBarWrapper").show()
        $("#progressBar").progressbar("enable");
        $.ajax({
            type: "POST",
            url: "/Home/GetDataTable" + qs,
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: true,
            success: function (tableSet) {
                $("#progressBarWrapper").hide()
                $("#progressBar").progressbar("disable");
                $('#divDataTableContainer').append(tableSet);
            },
            error: function (message) {
                console.error(message.responseText);
            }
        })
    }
}

function Validate(x) {
    var reg = RegExp("^\\$?(\\d{1,3},?(\\d{3},?)*\\d{3}(\\.\\d{1,3})?|\\d{1,3}(\\.\\d{2})?)$");
    var y = reg.test(x);
    return !y;
}

function AlphaStrip(x) {
    var dollarSign = RegExp("\\$");
    var comma = RegExp(",");
    var y = x.replace(dollarSign, "");
    var y = y.replace(comma, "");
    return y;
}

function UpdatePayments(i, v) {
    $("span.columnVisibility").hide();
    $.ajax({
        type: "GET",
        url: "/Home/UpdatePayments?i=" + i + "&v=" + v,
        async: true,
        success: function (tableSet) {
            $('#divDataTableContainer').append(tableSet);
        },
        error: function (message) {
            console.error(message.responseText);
        }
    })

}

function GetPymt(i) {
    return $("#txtAddToPrinciple-" + i).val()
}
