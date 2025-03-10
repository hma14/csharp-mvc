
$(function () {


    $("#nav").addClass("js").before('<div id="menu"><i class="fa fa-bars"></i></div>');
    $("#menu").click(function () {
        $("#nav").toggle();
    });
    $(window).resize(function () {
        if (window.innerWidth > 768) {
            $("#nav").removeAttr("style");
        }
    });

    $("#secondnav").addClass("js").before('<div id="submenu"><i class="fa fa-bars"></i></div>');
    $("#submenu").click(function () {
        $("#secondnav").toggle();
    });
    $(window).resize(function () {
        if (window.innerWidth > 768) {
            $("#secondnav").removeAttr("style");
        }
    });

    $('#search input').keydown(function (e) {
        if (e.keyCode === 13) {
            e.preventDefault();
            $("#search input[value='OK']").focus().click();
            return false;
        }
    });

    $('.tabs-wrapper .nav-link').click(function () {
        $('.tabs-wrapper .nav-link').css('background-color', '#3f51b5');
        $('.tabs-wrapper .nav-link').css('color', '#ffff');
        $(this).css('background-color', '#ffff');
        $(this).css('color', 'black');
    });


    $('.wrapper').mouseenter(

        function () {
            alert('enter');
            $(this).find(".card-image").hide();
            $(this).find(".my-card-text").show();
        },
        function () {
            $(this).find(".card-image").hide();
            $(this).find(".my-card-text").show();
        }
    );

    $('.wrapper').mouseleave(
        function () {
            alert('leave');
            $(this).find(".my-card-text").hide();
            $(this).find(".card-image").show();
        },
        function () {
            $(this).find(".my-card-text").hide();
            $(this).find(".card-image").show();
        }
    );



    $('.my-tag').on('click', function (event) {

        event.preventDefault();
        var $taskId = $(this).data('id1');
        //var $node = $(this).find('i');
        var $node = $(this);
        var $url = '//' + window.location.host + '/TaskDatas/UpdateTag';
        

        $.ajax({
            //url: '@Url.Action("UpdateTag", "TaskDatas")',
            url: $url,
            type: 'POST',
            data: addRequestVerificationToken({ taskId: $taskId }),
            success: function (result) {
                if ($node.hasClass('green-text') === true) {
                    $node.removeClass('green-text');
                }
                else {
                    $node.addClass('green-text');
                }
            },
            error: function (e) {
                alertify.alert(e.responseText);
            }
        });
        return false;
    });

    $('.show1').click(function () {
        $(this).parent().parent().parent().find('#list1').fadeIn('slow');
        $(this).hide();
        $('.hide1').show();
        $('.gridview1').show();
        $('#action-indicator1').css({ 'display': "none" });

    });

    $('.hide1').click(function () {
        $(this).parent().parent().parent().find('#list1').fadeOut('slow');
        $(this).parent().parent().parent().find('#grid1').fadeOut('slow');
        $(this).hide();
        $('.show1').show();
        $('.listview1').hide();
        $('.gridview1').hide();
        var $flag = $('#grid1').find('a').hasClass('action-flag');
        if ($flag === true) {
            $('#action-indicator1').css({ 'display': "inline-block" });
        };
    });

    $('.gridview1').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.listview1');
            $(this).parent().parent().parent().find('#list1').fadeOut('slow');
            $(this).parent().parent().parent().find('#grid1').fadeIn('slow');
            $('.gridview1').hide();
            $('.listview1').show();
        });
    $('.listview1').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.gridview1');
            $(this).parent().parent().parent().find('#grid1').fadeOut('slow');
            $(this).parent().parent().parent().find('#list1').fadeIn('slow');
            $('.listview1').hide();
            $('.gridview1').show();
        });


    $('.show2').click(function () {
        $(this).parent().parent().parent().find('#list2').fadeIn('slow');
        $(this).hide();
        $('.hide2').show();
        $('.gridview2').show();
        $('#action-indicator2').css({ 'display': "none" });

    });

    $('.hide2').click(function () {
        $(this).parent().parent().parent().find('#list2').fadeOut('slow');
        $(this).parent().parent().parent().find('#grid2').fadeOut('slow');
        $(this).hide();
        $('.show2').show();
        $('.listview2').hide();
        $('.gridview2').hide();
        var $flag = $('#grid2').find('a').hasClass('action-flag');
        if ($flag === true) {
            $('#action-indicator2').css({ 'display': "inline-block" });
        };
    });


    $('.gridview2').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.listview2');
            $(this).parent().parent().parent().find('#list2').fadeOut('slow');
            $(this).parent().parent().parent().find('#grid2').fadeIn('slow');
            $('.gridview2').hide();
            $('.listview2').show();
        });

    $('.listview2').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.gridview2');
            $(this).parent().parent().parent().find('#grid2').fadeOut('slow');
            $(this).parent().parent().parent().find('#list2').fadeIn('slow');
            $('.listview2').hide();
            $('.gridview2').show();

        });

    $('.show3').click(function () {
        $(this).parent().parent().parent().find('#list3').fadeIn('slow');
        $(this).hide();
        $('.hide3').show();
        $('.gridview3').show();
        $('#action-indicator3').css({ 'display': "none" });
    });

    $('.hide3').click(function () {
        $(this).parent().parent().parent().find('#list3').fadeOut('slow');
        $(this).parent().parent().parent().find('#grid3').fadeOut('slow');
        $(this).hide();
        $('.show3').show();
        $('.listview3').hide();
        $('.gridview3').hide();
        var $flag = $('#grid3').find('a').hasClass('action-flag');
        if ($flag === true) {
            $('#action-indicator3').css({ 'display': "inline-block" });
        };
    });


    $('.gridview3').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.listview3');
            $(this).parent().parent().parent().find('#list3').fadeOut('slow');
            $(this).parent().parent().parent().find('#grid3').fadeIn('slow');
            $('.gridview3').hide();
            $('.listview3').show();
        });

    $('.listview3').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.gridview3');
            $(this).parent().parent().parent().find('#grid3').fadeOut('slow');
            $(this).parent().parent().parent().find('#list3').fadeIn('slow');
            $('.listview3').hide();
            $('.gridview3').show();
        });

    //$('.show4').click(function () {
    //    $(this).parent().parent().parent().find('#list4').fadeIn('slow');
    //    $(this).hide();
    //    $('.hide4').show();
    //    $('.gridview4').show();
    //    $('#action-indicator4').css({ 'display': "none" });
    //});

    //$('.hide4').click(function () {
    //    $(this).parent().parent().parent().find('#list4').fadeOut('slow');
    //    $(this).parent().parent().parent().find('#grid4').fadeOut('slow');
    //    $(this).hide();
    //    $('.show4').show();
    //    $('.listview4').hide();
    //    $('.gridview4').hide();
    //    var $flag = $('#grid4').find('a').hasClass('action-flag');
    //    if ($flag === true) {
    //        $('#action-indicator4').css({ 'display': "inline-block" });
    //    };
    //});

    //$('.gridview4').click(
    //    function (e) {
    //        e.preventDefault();

    //        $otherNode = $(this).parent().find('.listview4');
    //        $(this).parent().parent().parent().find('#list4').fadeOut('slow');
    //        $(this).parent().parent().parent().find('#grid4').fadeIn('slow');
    //        $('.gridview4').hide();
    //        $('.listview4').show();

    //    });
    //$('.listview4').click(
    //    function (e) {
    //        e.preventDefault();

    //        $otherNode = $(this).parent().find('.gridview4');
    //        $(this).parent().parent().parent().find('#grid4').fadeOut('slow');
    //        $(this).parent().parent().parent().find('#list4').fadeIn('slow');
    //        $('.listview4').hide();
    //        $('.gridview4').show();
    //    });

    $('.show5').click(function () {
        $(this).parent().parent().parent().find('#list5').fadeIn('slow');
        $(this).hide();
        $('.hide5').show();
        $('.gridview5').show();
        $('#action-indicator5').css({ 'display': "none" });

    });

    $('.hide5').click(function () {
        $(this).parent().parent().parent().find('#list5').fadeOut('slow');
        $(this).parent().parent().parent().find('#grid5').fadeOut('slow');
        $(this).hide();
        $('.show5').show();
        $('.listview5').hide();
        $('.gridview5').hide();
        var $flag = $('#grid5').find('a').hasClass('action-flag');
        if ($flag === true) {
            $('#action-indicator5').css({ 'display': "inline-block" });
        };
    });


    $('.gridview5').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.listview5');
            $(this).parent().parent().parent().find('#list5').fadeOut('slow');
            $(this).parent().parent().parent().find('#grid5').fadeIn('slow');
            $('.gridview5').hide();
            $('.listview5').show();
        });

    $('.listview5').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.gridview5');
            $(this).parent().parent().parent().find('#grid5').fadeOut('slow');
            $(this).parent().parent().parent().find('#list5').fadeIn('slow');
            $('.listview5').hide();
            $('.gridview5').show();

        });

    $('.order-gridview').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.order-listview');
            $(this).parent().parent().parent().find('#order-list').fadeOut('slow');
            $(this).parent().parent().parent().find('#order-grid').fadeIn('slow');
            $(this).removeClass('green-text');
            $(this).addClass('pink-text');
            $otherNode.removeClass('pink-text');
            $otherNode.addClass('green-text');

        });
    $('.order-listview').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.order-gridview');
            $(this).parent().parent().parent().find('#order-grid').fadeOut('slow');
            $(this).parent().parent().parent().find('#order-list').fadeIn('slow');
            $(this).removeClass('green-text');
            $(this).addClass('pink-text');
            $otherNode.removeClass('pink-text');
            $otherNode.addClass('green-text');
        });

    $('.order-gridview2').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.order-listview2');
            $(this).parent().parent().parent().find('#order-list2').fadeOut('slow');
            $(this).parent().parent().parent().find('#order-grid2').fadeIn('slow');
            $(this).removeClass('green-text');
            $(this).addClass('pink-text');
            $otherNode.removeClass('pink-text');
            $otherNode.addClass('green-text');

        });
    $('.order-listview2').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.order-gridview2');
            $(this).parent().parent().parent().find('#order-grid2').fadeOut('slow');
            $(this).parent().parent().parent().find('#order-list2').fadeIn('slow');
            $(this).removeClass('green-text');
            $(this).addClass('pink-text');
            $otherNode.removeClass('pink-text');
            $otherNode.addClass('green-text');
        });


    $('.product-gridview').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.product-listview');
            $(this).parent().parent().parent().find('#product-list').fadeOut('slow');
            $(this).parent().parent().parent().find('#product-grid').fadeIn('slow');
            $(this).removeClass('green-text');
            $(this).addClass('pink-text');
            $otherNode.removeClass('pink-text');
            $otherNode.addClass('green-text');

        });
    $('.product-listview').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.product-gridview');
            $(this).parent().parent().parent().find('#product-grid').fadeOut('slow');
            $(this).parent().parent().parent().find('#product-list').fadeIn('slow');
            $(this).removeClass('green-text');
            $(this).addClass('pink-text');
            $otherNode.removeClass('pink-text');
            $otherNode.addClass('green-text');
        });

    $('.alerts-gridview').click(
        function (e) {
            e.preventDefault();
            $otherNode = $(this).parent().find('.alerts-listview');
            $(this).parent().parent().parent().find('#alerts-list').fadeOut('slow');
            $(this).parent().parent().parent().find('#alerts-grid').fadeIn('slow');
            $(this).removeClass('green-text');
            $(this).addClass('pink-text');
            $otherNode.removeClass('pink-text');
            $otherNode.addClass('green-text');

        });
    $('.alerts-listview').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.alerts-gridview');
            $(this).parent().parent().parent().find('#alerts-grid').fadeOut('slow');
            $(this).parent().parent().parent().find('#alerts-list').fadeIn('slow');
            $(this).removeClass('green-text');
            $(this).addClass('pink-text');
            $otherNode.removeClass('pink-text');
            $otherNode.addClass('green-text');
        });

    var $flag = $('a').hasClass('action-flag');
    if ($flag === true) {
        $('#top-action-flag').css({ 'display': "inline-block", 'font-size': "0.8em", 'vertical-align': "top" });
    }



    $('.navbar-nav li').click(function (e) {
        //e.preventDefault();
        $('navbar-nav li.active').removeClass('active');
        $(this).addClass('active');

    });




    // Registration
    var country = $("#country").val();
    $("#postalcode").val("");
    $("#zipcode").val("");
    $("#areacode").val("");

    if (country === "236") { // usa
        $(".usa").show(200);
        $(".canada").hide(200);
        $("#postalcode").val("");
        $("#zipcode").show(200);
    }
    else if (country === "40") { // canada
        $(".canada").show(200);
        $(".usa").hide(200);
        $("#zipcode").val("");
        $("#postalcode").show(200);
    }
    else { // other countries
        $(".others").show(200);
        $(".canada").hide(200);
        $(".usa").hide(200);
        $("#zipcode").val("");
        $("#postalcode").val("");
        $("#areacode").show(200);
    }

    $("#country").change(function () {

        $(".canada").hide(200);
        $(".usa").hide(200);
        $(".others").hide(200);
        $("#postalcode").val("");
        $("#areacode").val("");
        $("#zipcode").val("");

        var val = $(this).val();

        if (val === "236") { // usa
            $(".usa").show(200);
            $("#postalcode").val("");
            $("#postalcode").hide();
            $("#areacode").hide();
            $("#zipcode").show(200);
        }
        else if (val === "40") {
            $(".canada").show(200);
            $("#zipcode").val("");
            $("#zipcode").hide();
            $("#areacode").hide();
            $("#postalcode").show(200);
        }
        else {
            $(".others").show(200);
            $("#postalcode").val("");
            $("#zipcode").val("");
            $("#areacode").show(200);
        }
    });

    $('.quantity').focusout(function () {

        var qty1 = $('#qty1').val();
        var qty2 = $('#qty2').val();
        var qty3 = $('#qty3').val();
        var qty4 = $('#qty4').val();
        var qty5 = $('#qty5').val();
        var qty6 = $('#qty6').val();
        var qty7 = $('#qty7').val();

        if (qty1 !== null && qty2 !== null && parseInt(qty1) >= parseInt(qty2)) {
            $(this).css("background-color", "#ff4444").css("color", "white");
            alertify.error('qty 2 must be bigger than qty 1!');
            return false;
        }
        else {
            $(this).css("background-color", "#ffffff").css("color", "black");
        }

        if (qty2 !== null && qty3 !== null && parseInt(qty2) >= parseInt(qty3)) {
            $(this).css("background-color", "#ff4444").css("color", "white");
            alertify.error('qty 3 must be bigger than qty 2!');
            return false;
        }
        else {
            $(this).css("background-color", "#ffffff").css("color", "black");
        }

        if (qty3 !== null && qty4 !== null && parseInt(qty3) >= parseInt(qty4)) {
            $(this).css("background-color", "#ff4444").css("color", "white");
            alertify.error('qty 4 must be bigger than qty 3!');
            return false;
        }
        else {
            $(this).css("background-color", "#ffffff").css("color", "black");
        }
        if (qty4 !== null && qty5 !== null && parseInt(qty4) >= parseInt(qty5)) {
            $(this).css("background-color", "#ff4444").css("color", "white");
            alertify.error('qty 5 must be bigger than qty 4!');
            return false;
        }
        else {
            $(this).css("background-color", "#ffffff").css("color", "black");
        }
        if (qty5 !== null && qty6 !== null && parseInt(qty5) >= parseInt(qty6)) {
            $(this).css("background-color", "#ff4444").css("color", "white");
            alertify.error('qty 6 must be bigger than qty 5!');
            return false;
        }
        else {
            $(this).css("background-color", "#ffffff").css("color", "black");
        }
        if (qty6 !== null && qty7 !== null && parseInt(qty6) >= parseInt(qty7)) {
            $(this).css("background-color", "#ff4444").css("color", "white");
            alertify.error('qty 7 must be bigger than qty 6!');
            return false;
        }
        else {
            $(this).css("background-color", "#ffffff").css("color", "black");
        }
    });

    var $flag4 = $('#grid4').find('a').hasClass('action-flag');
    if ($flag4 === true) {
        $('#action-indicator4').css({ 'display': "inline-block" });
    };


    $('.show4').click(function () {

        if ($('#more').find('#list4').length === 0) {

            var $url = '//' + window.location.host + '/Home/PartialVendorCompleteProducts';
            

            $('#spinner').show();
            $.ajax({
                type: 'GET',
                url: $url,
                success: function (result) {
                    $('#spinner').hide();
                    $('#more').html(result);

                    $('#more').find('#list4').fadeIn('slow');
                },
                error: function (error) {
                    $('#spinner').hide();
                    alertify.error(error.statusCode);
                }
            });
        }

        $('#more').find('#list4').fadeIn('slow');
        $(this).hide();
        $('.hide4').show();
        $('.gridview4').show();
        $('#action-indicator4').css({ 'display': "none" });
    });

    $('.hide4').click(function () {
        $(this).parent().parent().parent().find('#list4').fadeOut('slow');
        $(this).parent().parent().parent().find('#grid4').fadeOut('slow');
        $(this).hide();
        $('.show4').show();
        $('.listview4').hide();
        $('.gridview4').hide();
        var $flag = $('#grid4').find('a').hasClass('action-flag');
        if ($flag === true) {
            $('#action-indicator4').css({ 'display': "inline-block" });
        };
    });

    $('.gridview4').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.listview4');
            $(this).parent().parent().parent().find('#list4').fadeOut('slow');
            $(this).parent().parent().parent().find('#grid4').fadeIn('slow');
            $('.gridview4').hide();
            $('.listview4').show();

        });
    $('.listview4').click(
        function (e) {
            e.preventDefault();

            $otherNode = $(this).parent().find('.gridview4');
            $(this).parent().parent().parent().find('#grid4').fadeOut('slow');
            $(this).parent().parent().parent().find('#list4').fadeIn('slow');
            $('.listview4').hide();
            $('.gridview4').show();
        });

    //$('#EstimateCompletionDate').datepicker();
});


function isRadioButtonChecked() {
    $('#spinner').show();
    var chosen_one = false;
    $('input:radio').each(function () {

        var value = $(this).attr("value");

        if (value === 'NCRArbitrateDispute' || value === 'NCRCustomerRevision' || value === 'NCRCorrectivePartsReceival' ||
            value === 'RevisingRFQ' || value === 'ReadyForTooling' || value === 'ToolingComplete' ||
            value === 'CompleteSample' || value === 'InProduction' || value === 'CompleteProduction' ||
            value === 'OrderPaid' || value === 'StartedProof' || value === 'PendingReorderPaymentMade' ||
            value === 'ProofApproval' || value === 'ReadyForTooling' || value === 'PendingPaymentMade' ||
            value === 'CorrectingProof' || value === 'CorrectingSample' || value === 'ReviewRFQBid' ||
            value === 'SetupUnitPricesForExtraQuantities' || value === 'PaidReOrder' ||
            value === 'CompleteInvoiceForVendor' || value === 'CreateInvoiceForVendor' ||
            value === 'NCRCorrectivePartsComplete' || value === 'NCRClose') {


            $("input[name=group][value=" + value + "]").prop('checked', true);
            chosen_one = true;
            return true;
        }

        var name = $(this).attr("name");

        //Checking whether radio button is selected and based on selection setting variable to true or false
        if ($("input:radio[name=" + name + "]:checked").length !== 0) {
            chosen_one = true;
            return true;
        }
    });
    if (chosen_one === false) {
        alertify.error("You need to choose one of actions before submit.");
        $('#spinner').hide();
        return false;
    }
};



function ShowModalView(taskId, orderId) {

    var $url = '//' + window.location.host + '/Home/ShowModal';
    
    $('#spinner').show();

    $.ajax({
        type: 'GET',
        url: $url,
        data: { 'Id': taskId, 'TaskId': taskId, 'OrderId': orderId},
        success: function (result) {           
            $('#spinner').hide();
            $('#Modal' + taskId).html(result);
            $('#Modal' + taskId).modal('show');
        },
        error: function(error) {
            $('#spinner').hide();
            alertify.error(error.statusCode);
        }
    });
};



function UploadProofFile(productId, taskId) {
    var $url = '//' + window.location.host + '/Documents/UploadProofing';
   

    $('#spinner').show();
    $.ajax({
        type: 'POST',
        url: $url,
        data: { 'productId': productId, 'taskId': taskId },
        success: function (result) {
            $('#spinner').hide();
            $('#Modal' + taskId).html(result);
            $('#Modal' + taskId).modal('show');
        },
        error: function (error) {
            $('#spinner').hide();
            alertify.error(error.statusCode);
        }
    });

}



function addRequestVerificationToken(data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};


function checkForm(form) {
    //
    // validate form fields
    //

    form.submitButton.disabled = true;
    return true;
};



