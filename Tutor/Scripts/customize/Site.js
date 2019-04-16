function SearchMenu_button_Event(ID, type) {
    var color = $("#" + ID).css("background-color")
    if (type == "Salary" || type == "gender") {
        $("." + type + "_class").css("background-color", '');
        $("#" + ID).css("background-color", '#F5AC00');
        $("#" + type).val(ID);
    }
    else {
        var typeStr = $("#" + type).val();
        if (color == "rgb(221, 221, 221)") {
            $("#" + ID).css("background-color", '#F5AC00');
            $("#" + type).val(typeStr + "," + ID);
        }
        else {
            $("#" + ID).css("background-color", 'rgb(221,221,221)');
            if (typeStr.indexOf("," + ID) >= 0)
                $("#" + type).val(typeStr.replace("," + ID, ""));
        }
    }
}
function Clear(Type) {
    if (Type == "ALL") {
        $(".subject_class").css("background-color", '');
        $("#subject").val("");
        $(".grade_class").css("background-color", '');
        $("#grade").val("");
        $(".Salary_class").css("background-color", '');
        $("#Salary").val("");
        $(".gender_class").css("background-color", '');
        $("#gender").val("");
    }
    else {
        $("." + Type + "_class").css("background-color", '');
        $("#" + Type).val("");
    }
}
function BindView(data, key) {

    BindValue(data, key);

    BindHiddenToLabel();


    $('input[data-to]').each(function () {
        try {
            this.change();
        } catch (e) {
        }
    });
    if (data.ErrorMsg != null && data.ErrorMsg != "") {
        alert(data.ErrorMsg);
        data.ErrorMsg = "";
    }
}


function BindValue(data) {

    for (var key in data) {

        if ($("label[id='" + key + "']").length > 0) {
            $("#" + key).text(data[key]);
        }
        else if ($("input[type=radio][name='" + key + "']").length > 0) {
            $("input[type=radio][name='" + key + "']").each(function () {
                if (data[key] != null) {
                    if (data[key] == $("#" + this.id).val()) {
                        $("#" + this.id).prop('checked', true);
                    }
                    else {
                        $("#" + this.id).prop('checked', false);
                    }
                }
                else {
                    $("#" + this.id).prop('checked', false);
                }
            });
        }
        else if ($("input[type=checkbox][name='" + key + "']").length > 0) {

            $("input[type=checkbox][name='" + key + "']").each(function () {
                if (data[key] != null) {
                    if (data[key].indexOf($("#" + this.id).val(), 0) > -1) {
                        $("#" + this.id).prop('checked', true);
                    }
                    else {
                        $("#" + this.id).prop('checked', false);
                    }
                }
                else {
                    $("#" + this.id).prop('checked', false);
                }
            });

        }
        else {
            $("#" + key).val(data[key]);
        }
    }
}
function BindHiddenToLabel() {
    $('input[type="hidden"]').each(function () {
        var targetLabelName = this.id;
        var getNameId = 'label[for=\'' + targetLabelName + '\']';
        if ($(getNameId).length > 0)
            $(getNameId).text($(this).val());
    });
}