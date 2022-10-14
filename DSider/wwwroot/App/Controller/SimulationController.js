var map;
var marker;
var $selectedNodeForContext;
var id = document.getElementById("drawflow");
const editor = new Drawflow(id);
var subProjectID = "";
var cntrlIsPressed = false;
var $mSelectedNodeFordblClick;
var modelType = "";
var workspaceZoomLevel = "1";
var workspaceTranslateX = "0";
var workspaceTranslateY = "0";
//When page load
jQuery(document).ready(function () {
  jQuery(
    "#btnApproveSharedProject,.Carbon_MitigationLevel2,.Green_HydrogenModel,.Carbon_MitigationLevel1,.realEstateLevel,.GeoThermalLevel"
  ).hide();
  jQuery("#MacroList").hide();
  subProjectID = getParameterByName("project");
  jQuery(".detailsLevel,.detailsLevel2").hide();
  jQuery("#projectNameContainer").text(getParameterByName("name"));
  //
  modelType = getParameterByName("type");
  if (modelType == "Green_Hydrogen") {
    jQuery(".Green_HydrogenModel").show().css("display", "inline-block");
    jQuery("#ddIconTypes option[value=Green_HydrogenModel]").prop(
      "selected",
      true
    );
  } else if (modelType == "Carbon_Mitigation") {
    jQuery(".Carbon_MitigationLevel1").show().css("display", "inline-block");
    jQuery("#ddIconTypes option[value=Carbon_MitigationLevel1]").prop(
      "selected",
      true
    );
  }

  //
  getexportData(0);
  //getSubProjectDetails(subProjectID);
  getComponentsProperties();
  getDataSources();
  getUserDefinedObjects();
  getUserMacros();
  checkPermissionSharedProject();
  //
  jQuery("#navs li").hide();
  setTimeout(function () {
    jQuery("#headingOne").find("a").trigger("click");
  }, 100);
  var ul = $("#navs"),
    li = $("#navs li"),
    i = li.length,
    n = i - 1,
    r = 120;
  //When click on simulatuin button
  jQuery(document).on("click", "#btnSimulate", function () {
    var modelInfo = saveModel(1);
    console.log("Posting data:", modelInfo);
    var redirectURL = "";
    var mDash = "Dashboard";
    if (getParameterByName("type") == "Carbon_Mitigation")
      mDash = "DashboardMitigation";
    redirectURL =
      "/Home/" +
      mDash +
      "/" +
      subProjectID +
      "/" +
      getParameterByName("name") +
      "/" +
      getParameterByName("folder") +
      "/" +
      getParameterByName("type");
    $.ajax({
        url: "http://127.0.0.1:4567/api/simulate/" + subProjectID,
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: JSON.stringify(modelInfo),
      success: function (response) {
        alertify.success("Simulation succeed.");
        setTimeout(function () {
          location.href = redirectURL;
        }, 12000);
      },
      error: function (response) {
        alertify.error("Simulation failed.");
        setTimeout(function () {
         location.href = redirectURL;
        }, 4000);
      },
      failure: function (response) {
        alertify.error("Simulation failed.");
      },
    });
  });
  //When right click on node in drawflow
  jQuery(document).on("contextmenu", ".drawflow-node", function (e) {
    $selectedNodeForContext = jQuery(this);
    var selectedID = jQuery(this).attr("id");
    var mNode = editor.getNodeFromId(selectedID.replace("node-", ""));
    var pos = jQuery(this).position();
    e.preventDefault();
    var width = jQuery(this).outerWidth();
    //jQuery("#navs").css({
    //    top: (pos.top + 150) + "px",
    //    left: (pos.left + width + 230) + "px"
    //});
    jQuery(this).toggleClass("active");
    if (jQuery(this).hasClass("active")) {
      jQuery("#navs li").show();
      for (var a = 0; a < i; a++) {
        li.eq(a).css({
          "transition-delay": "" + 50 * a + "ms",
          "-webkit-transition-delay": "" + 50 * a + "ms",
          left: r * Math.cos((90 / n) * a * (Math.PI / 180)),
          top: -r * Math.sin((90 / n) * a * (Math.PI / 180)),
        });
      }
      jQuery(this).toggleClass("active");
    } else {
      li.removeAttr("style");
      jQuery("#navs li").hide(50);
    }
  });
  //Close ContextMenu
  jQuery(document).on("click", "#closeMenu", function () {
    closeContextMenu();
  });
  //When click on show property on context menu
  jQuery(document).on("click", "#propertyMenu", function (e) {
    e.preventDefault();
    //getDataSources();
    showPropertyValuesByNodeID($selectedNodeForContext.attr("id"));
    //
    jQuery("#mPropertyPanel").addClass("cd-panel--is-visible");
    closeContextMenu();
  });
  //When click on fullscreen button on property panel
  let screenMode = "default";
  jQuery(document).on("click", "#fullscreen", function (e) {
    e.preventDefault();
    if (screenMode == "default") {
      jQuery(".cd-panel__container").css("width", "100%");
      jQuery(".cd-panel__header").css("width", "100%");
      jQuery("#fullscreen").css("margin-top", "-10px");
      screenMode = "full";
      jQuery("#fullscreen").html(
        '<i class="fa fa-window-minimize" aria-hidden="true" style="font-size:25px;"></i>'
      );
    } else {
      jQuery(".cd-panel__container").css("width", "50%");
      jQuery(".cd-panel__header").css("width", "50%");
      jQuery("#fullscreen").css("margin-top", "0px");
      screenMode = "default";
      jQuery("#fullscreen").html(
        '<i class="fa fa-window-maximize" aria-hidden="true" style="font-size:25px;"></i>'
      );
    }
  });
  //When click on delete on context menu
  jQuery(document).on("click", "#deleteMenu", function () {
    var curerrentComponentID = $selectedNodeForContext.attr("id");
    var mID = curerrentComponentID.replace("node-", "");
    editor.removeNodeId($selectedNodeForContext.attr("id"));
    jQuery("#mPropertyPanel").removeClass("cd-panel--is-visible");
    closeContextMenu();
  });
  //When click on duplicate on context menu
  jQuery(document).on("click", "#duplicateComponent", function () {
    var curerrentComponentID = $selectedNodeForContext.attr("id");
    var inputCount = $selectedNodeForContext.find(".input").length;
    var outputCount = $selectedNodeForContext.find(".output").length;

    var mID = curerrentComponentID.replace("node-", "");
    var infoOfCurrentNode = editor.getNodeFromId(mID);

    alertify.confirm("Are you sure to duplicate?", function (e) {
      if (e) {
        var mPostFix = createPostiFixForComponent(infoOfCurrentNode.class);
        var newID = editor.addNode(
          infoOfCurrentNode.name,
          inputCount,
          outputCount,
          parseFloat(infoOfCurrentNode.pos_x) + 20,
          parseFloat(infoOfCurrentNode.pos_y) + 20,
          infoOfCurrentNode.class,
          infoOfCurrentNode.data,
          infoOfCurrentNode.html
        );
        //
        //Update Name
        var mProperties = {};
        if (typeof infoOfCurrentNode.data.length != "undefined") {
          mProperties = JSON.parse(infoOfCurrentNode.data);
        }
        if (mProperties.hasOwnProperty("Name")) {
          //if before added this property we should edit this propery
          mProperties["Name"] = infoOfCurrentNode.name + mPostFix;
        }
        editor.updateNodeDataFromId(newID, JSON.stringify(mProperties));
      }
    });
    closeContextMenu();
  });
  //Where leave input items in property panel
  jQuery(document).on("focusout", ".propertyItem", function () {
    var curerrentComponentID = $selectedNodeForContext.attr("id");
    var mID = curerrentComponentID.replace("node-", "");
    var property = jQuery(this).attr("property");
    var propertyValue = jQuery(this).val();
    var infoOfCurrentNode = editor.getNodeFromId(mID);
    var mProperties = {};
    if (typeof infoOfCurrentNode.data.length != "undefined") {
      mProperties = JSON.parse(infoOfCurrentNode.data);
    }
    if (mProperties.hasOwnProperty(property)) {
      //if before added this property we should edit this propery
      mProperties[property] = propertyValue;
    } else {
      //if this property didn't add before
      mProperties[property] = propertyValue;
    }
    editor.updateNodeDataFromId(mID, JSON.stringify(mProperties));
    //
    if (property == "Name") {
      jQuery("#node-" + mID)
        .find(".drawflow_content_node")
        .find(".componentTitle")
        .text(propertyValue);
      editor.updateHtml(
        mID,
        jQuery("#node-" + mID)
          .find(".drawflow_content_node")
          .html()
      );
    }
  });
  //When click on save button, all components and json template will be save
  jQuery(document).on("click", "#btnSave", function () {
    if (jQuery(this).attr("disabled") == "disabled") return;
    if (jQuery(".detailsLevel").css("display") == "none") saveModel(1);
    else {
      saveComponentDetailsModel(false);
    }
  });
  //When click on map location button in property menu
  jQuery(document).on("click", "#locationIcon", function () {
    var curerrentComponentID = $selectedNodeForContext.attr("id");
    var mID = curerrentComponentID.replace("node-", "");
    var infoOfCurrentNode = editor.getNodeFromId(mID);
    var mProperties = {};
    if (typeof infoOfCurrentNode.data.length != "undefined") {
      mProperties = JSON.parse(infoOfCurrentNode.data);
      for (var key in mProperties) {
        if (key == "location")
          jQuery('.propertyItem[property="location"]').val(mProperties[key]);
      }
    }
    setTimeout(function () {
      if (jQuery('.propertyItem[property="location"]').val() != "") {
        var lat = parseFloat(
          jQuery('.propertyItem[property="location"]').val().split(",")[0]
        );
        var lng = parseFloat(
          jQuery('.propertyItem[property="location"]').val().split(",")[1]
        );
        marker.setPosition(parsedPosition);
      }
    }, 100);
    jQuery("#locationModal").modal("show");
  });
  //When modal is getting close
  jQuery("#locationModal").on("hidden.bs.modal", function () {
    jQuery('.propertyItem[property="location"]').trigger("focusout");
  });
  //When click on page context menu will be close
  jQuery(document).on("click", ".col-right", function (e) {
    closeContextMenu();
  });
  //When click on page all node connections color will be reset
  jQuery(document).on("click", function (e) {
    if (
      e.target.className != "iDesign" &&
      e.target.className != "title-box" &&
      e.target.className != "componentTitle"
    )
      jQuery(".connection").find("path").css("stroke", "#4ea9ff");
    //
    $(".context2").fadeOut("fast");
  });
  //When click on add new datasource icon in DataSource section in left bar
  jQuery(document).on("click", "#btnAddNewDataSources", function () {
    jQuery("#txtDataSourceName,#txtDataSourceDescription,#files").val("");
    jQuery("#dataSourceModal").modal("show");
  });
  //When click on delete icon in datasource list table
  jQuery(document).on("click", ".removeDataSource", function () {
    var mID = jQuery(this).attr("mID");
    alertify.confirm("Are you sure to delete?", function (e) {
      if (e) {
        $.ajax({
          url: "/api/WebAPI_DataSource/deleteDataSource/" + mID,
          type: "GET",
          contentType: "application/json",
          success: function (response) {
            alertify.log("Item deleted.");
            getDataSources();
          },
          error: function (response) {},
          failure: function (response) {},
        });
      }
    });
  });
  var selectedProperyForBrowse = "";
  //When select a datasource
  jQuery(document).on("click", ".btnBrowseDataSource", function () {
    selectedProperyForBrowse = jQuery(this).attr("property");
    //
    jQuery("#dataSourceList").empty();
    jQuery(dataSourcesInfo).each(function (Index, Val) {
      var htmlCode = "<li>";
      htmlCode += '<input id="group-' + Index + '" type="checkbox" hidden />';
      htmlCode +=
        '<label for="group-' +
        Index +
        '"><span class="fa fa-angle-right"></span> ' +
        Val.name +
        "</label>";
      htmlCode += '<ul class="group-list">';
      jQuery(Val.columnsList).each(function (colIndex, colVal) {
        htmlCode +=
          '<li class="DataSourcecolumn" dataSource="' +
          Val.name +
          '" coloumn="' +
          colVal +
          '"><a href="#">' +
          colVal +
          "</a></li>";
      });
      htmlCode += "</ul></li>";
      jQuery("#dataSourceList").append(htmlCode);
    });
    //
    jQuery("#selectDataSourceForPropertyModal").modal("show");
  });
  //When click displayed columns of datasource
  jQuery(document).on("click", ".DataSourcecolumn", function () {
    var selectedColumn = jQuery(this).attr("coloumn");
    var selectedDataSourceForCol = jQuery(this).attr("dataSource");
    jQuery('.propertyItem[property="' + selectedProperyForBrowse + '"]')
      .val(selectedDataSourceForCol + " | " + selectedColumn)
      .trigger("focusout");
    //jQuery('.propertyItem[property="' + selectedProperyForBrowse+'"]')
    jQuery("#selectDataSourceForPropertyModal").modal("hide");
  });
  //Show all components in the map
  jQuery(document).on("click", "#sowAllComponentsOnMap", function () {
    jQuery("#allMarkersMapModal").modal("show");
    initializeAllComponentsMap();
  });
  //Export json file
  jQuery(document).on("click", "#btnExportJSONFile", function () {
    jQuery("#fakeHrefForDownload")
      .attr("href", "/Home/downloadExportJSON/" + subProjectID)
      .attr("target", "_blank");
    jQuery("#fakeHrefForDownload").trigger("click");
    return;
    $.ajax({
      url: "/api/WebAPI_Export/exportJSONFile/" + subProjectID,
      type: "GET",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        alertify.success("File has been downloaded.");
        fs.writeFile("test2.zip", data, { encoding: "base64" }, (err) => {
          if (err) throw err;
        });
      },
      error: function (response) {},
      failure: function (response) {},
    });
  });
  // Longitude validation
  jQuery("#longitudeInput").change(function () {
    var longitude = jQuery("#longitudeInput").val();
    if (Math.abs(longitude) <= 180) {
      jQuery("#longitudeFeedback").css("display", "none");
    } else {
      jQuery("#longitudeFeedback").css("display", "block");
    }
  });
  // Latitude Validation
  jQuery("#latitudeInput").change(function () {
    var latitude = jQuery("#latitudeInput").val();
    if (Math.abs(latitude) <= 90) {
      jQuery("#latitudeFeedback").css("display", "none");
    } else {
      jQuery("#latitudeFeedback").css("display", "block");
    }
  });
  //When click on delete icon in context menu to remove selected component
  jQuery(document).on("click", ".deleteContextMenu", function () {
    alertify.confirm("Are you sure to delete?", function (e) {
      if (e) {
        if (typeDelete == "userDefinedObjectIcons")
          $.ajax({
            url:
              "/api/WebAPI_UserDefinedObjects/deleteUserDefinedObject/" +
              selectedUserdefindeID,
            type: "GET",
            contentType: "application/json",
            success: function (response) {
              getUserDefinedObjects();
              alertify.log("Object Deleted.");
            },
            error: function (response) {},
            failure: function (response) {},
          });
        else if (typeDelete == "userMacroIcons")
          $.ajax({
            url: "/api/WebAPI_Macros/deleteMacroObject/" + selectedMacroID,
            type: "GET",
            contentType: "application/json",
            success: function (response) {
              getUserMacros();
              alertify.log("Macro Deleted.");
            },
            error: function (response) {},
            failure: function (response) {},
          });
      }
    });
  });
  //When CTRL button has been pressed
  jQuery(document).on("keydown", document, function () {
    cntrlIsPressed = true;
  });
  //When CTRL button has been released
  jQuery(document).on("keyup", document, function () {
    cntrlIsPressed = false;
  });
  //When click button for save macros
  jQuery(document).on("click", "#btnReadyForSaveMacro", function () {
    jQuery("#MacroModal").modal("show");
  });
  //When click on cancel in macros panel
  jQuery(document).on("click", "#btnCancelMacro", function () {
    alertify.confirm("Are you sure to cancel Macro?", function (e) {
      if (e) {
        doCancelMacro();
      }
    });
  });
  //Delete selected macro from the list
  jQuery(document).on("click", ".deleteFromMacroList", function () {
    var $selectedItem = jQuery(this);
    var mID = $selectedItem.parent().parent().parent().attr("nodeID");
    alertify.confirm("Are you sure to delete?", function (e) {
      if (e) {
        jQuery("#list")
          .find('li[nodeID="' + mID + '"]')
          .removeClass("show");
        jQuery("#node-" + mID)
          .find(".checkASMacro")
          .remove();
        setTimeout(function () {
          $selectedItem.parent().parent().parent().remove();
        }, 1000);
        if (jQuery("#list").find(".show").length == 0)
          jQuery("#MacroList").hide();
      }
    });
  });
  var selectedUserdefindeID = "";
  var selectedMacroID = "";
  var typeDelete = "";
  //When right click on user defined object
  jQuery(document).on(
    "contextmenu",
    ".userDefinedObjectIcons,.userMacroIcons",
    function (e) {
      if (jQuery(this).hasClass("userDefinedObjectIcons")) {
        selectedUserdefindeID = jQuery(this).attr("mID");
        typeDelete = "userDefinedObjectIcons";
      } else if (jQuery(this).hasClass("userMacroIcons")) {
        selectedMacroID = jQuery(this).attr("mID");
        typeDelete = "userMacroIcons";
      }
      jQuery(".context2")
        .show()
        .css({
          top: event.pageY - 70,
          left: event.pageX - 50,
        });
      return false;
    }
  );
  //When click on comment icon in context menu
  jQuery(document).on("click", "#commentComponent", function () {
    jQuery("#txtComponentComment").val("");
    jQuery("#ComponentCommentModal").modal("show");
    closeContextMenu();
  });
  //When click on comment icon on each component
  jQuery(document).on("click", ".componentComment", function () {
    var mNodeID = jQuery(this).attr("nodeID");
    var filteredCommets = allCompnentsComments.filter(function (el) {
      return el.nodeID === mNodeID;
    });
    var lastUser = "";
    var chatStatus = "";
    jQuery("#chatContent").empty();
    jQuery(filteredCommets).each(function (Index, Val) {
      var htmlCode = "";
      if (Val.registerUser != lastUser) {
        if (chatStatus == "") chatStatus = "chat-left";
        else chatStatus = "";
      }

      htmlCode +=
        '<div class="chat ' +
        chatStatus +
        '"><div class="chat-avatar"><a class="avatar" data-toggle="tooltip" href="#" data-placement="right" title="' +
        Val.registerUser +
        '">';
      htmlCode += '<img src="/Images/avatar.png" alt="June Lane"></a></div>';
      htmlCode += '<div class="chat-body">';
      htmlCode +=
        '<div class="chat-content"><div class="userChatInfo">' +
        Val.registerUser +
        "</div><p>" +
        Val.commentText +
        '</p><time class="chat-time" datetime="' +
        Val.registerDate +
        '">' +
        Val.registerDate +
        "</time>";
      htmlCode += "</div></div></div>";
      jQuery("#chatContent").append(htmlCode);
      lastUser = Val.registerUser;
    });
    //
    jQuery("#showCommentsModal").modal("show");
  });
  //When click change input/output node counts in context menu
  jQuery(document).on("click", "#connectionCount", function () {
    closeContextMenu();
    getConnectionsForModal();
    jQuery("#inputOutputModal").modal("show");
  });
  //Delete input/output node from the list
  jQuery(document).on("click", ".deleteFromInputOutputNodes", function () {
    var $selectedItem = jQuery(this);
    var mSelectedNodeForDelete = jQuery(this).attr("nodeID");
    var mID = $selectedNodeForContext.attr("id");
    mID = mID.replace("node-", "");
    alertify.confirm("Are you sure to delete?", function (e) {
      if (e) {
        $selectedItem.parent().parent().parent().removeClass("show");
        setTimeout(function () {
          $selectedItem.parent().parent().parent().remove();
        }, 1000);
        var mComponent = editor.getNodeFromId(mID);
        //
        //#region Input Connections
        if (mSelectedNodeForDelete.indexOf("input_") != -1) {
          editor.removeNodeInput(mID, mSelectedNodeForDelete);
          if (
            mComponent.inputs.hasOwnProperty("input_1") ||
            mComponent.inputs.hasOwnProperty("input_2")
          ) {
            for (var key in mComponent.inputs) {
              jQuery(mComponent.inputs[key].connections).each(function (
                index,
                Val
              ) {
                if (key == mSelectedNodeForDelete) {
                  editor.removeSingleConnection(
                    parseInt(Val.node),
                    parseInt(mID),
                    Val.input,
                    mSelectedNodeForDelete
                  );
                }
              });
            }
          }
          //$selectedNodeForContext.find('.inputs').find('.' + mSelectedNodeForDelete).remove();
          editor.updateConnectionNodes(mID);
        }
        //#endregion Input Connections
        //#region Output Connections
        if (mSelectedNodeForDelete.indexOf("output_") != -1) {
          editor.removeNodeOutput(mID, mSelectedNodeForDelete);
          if (
            mComponent.outputs.hasOwnProperty("output_1") ||
            mComponent.outputs.hasOwnProperty("output_2")
          ) {
            for (var key in mComponent.outputs) {
              jQuery(mComponent.outputs[key].connections).each(function (
                index,
                Val
              ) {
                if (key == mSelectedNodeForDelete) {
                  editor.removeSingleConnection(
                    parseInt(mID),
                    parseInt(Val.node),
                    mSelectedNodeForDelete,
                    Val.output
                  );
                }
              });
            }
          }
          //$selectedNodeForContext.find('.outputs').find('.' + mSelectedNodeForDelete).remove();
          editor.updateConnectionNodes(mID);
        }
        //#endregion Output Connections
      }
    });
  });
  //When double click on components
  //New sub level will be display
  jQuery(document).on("dblclick", ".drawflow-node", function () {
    jQuery("#ddIconTypes").val("");
    jQuery(
      ".RefineryLevel,.FleetLevel,.realEstateLevel,.Green_HydrogenModel,.GeoThermalLevel"
    ).hide();

    if (jQuery(".detailsLevel2").css("display") != "none") return;
    if (jQuery(".detailsLevel").css("display") == "none") saveModel(2);
    else saveComponentDetailsModel(false);
    //
    $mSelectedNodeFordblClick = jQuery(this);
    //
    if (jQuery(".detailsLevel").css("display") != "none")
      jQuery(".detailsLevel2").show();
    var mID = $mSelectedNodeFordblClick.attr("id");
    mID = mID.replace("node-", "");
    var mComponent = editor.getNodeFromId(mID);
    //#region Properties
    var mProperties = {};
    if (typeof mComponent.data.length != "undefined") {
      mProperties = JSON.parse(mComponent.data);
    }
    var mCurrentComponentName = "";
    for (var key in mProperties) {
      if (key == "Name") {
        mCurrentComponentName = mProperties[key];
        if (jQuery(".detailsLevel").css("display") != "none")
          //Level2
          jQuery("#Level2Container")
            .text(mProperties[key])
            .attr("currentNodeID", $mSelectedNodeFordblClick.attr("id"))
            .attr("componentType", mComponent.name);
        else
          jQuery("#detailsLevelContainer")
            .text(mProperties[key])
            .attr("currentNodeID", $mSelectedNodeFordblClick.attr("id"))
            .attr("componentType", mComponent.name);
      }
    }
    //#endregion Properties
    jQuery(".detailsLevel").show(); //title
    //
    if (jQuery(".detailsLevel").css("display") != "none") {
      jQuery(".Carbon_MitigationLevel2").show();
      jQuery(".Carbon_MitigationLevel1").hide();
    } else {
      jQuery(".Carbon_MitigationLevel2").hide();
      jQuery(".Carbon_MitigationLevel1").show();
    }
    //if refinery selected
    if (mCurrentComponentName.toLowerCase().indexOf("refinery") != -1) {
      jQuery(
        ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.GeoThermalLevel"
      ).hide();
      jQuery(".RefineryLevel").show().css("display", "inline-block");
    } else if (mCurrentComponentName.toLowerCase().indexOf("fleet") != -1) {
      jQuery(
        ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.GeoThermalLevel"
      ).hide();
      jQuery(".FleetLevel").show().css("display", "inline-block");
    } else if (mCurrentComponentName.toLowerCase().indexOf("real") != -1) {
      jQuery(
        ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.GeoThermalLevel"
      ).hide();
      jQuery(".realEstateLevel").show().css("display", "inline-block");
    } else if (mCurrentComponentName.toLowerCase().indexOf("green") != -1) {
      jQuery(
        ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.realEstateLevel,.GeoThermalLevel"
      ).hide();
      jQuery(".Green_HydrogenModel").show().css("display", "inline-block");
    } else if (
      mCurrentComponentName.toLowerCase().indexOf("geothermal") != -1
    ) {
      jQuery(
        ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.realEstateLevel,.Green_HydrogenModel"
      ).hide();
      jQuery(".GeoThermalLevel").show().css("display", "inline-block");
    }
  });
  //Change icon types for showing to user
  jQuery(document).on("change", "#ddIconTypes", function () {
    if (jQuery(this).val() == "") return;
    var mSelectedItem = jQuery(this).val();
    jQuery(
      ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.realEstateLevel,.Green_HydrogenModel,.GeoThermalLevel,.FleetLevel,.RefineryLevel"
    ).hide();
    jQuery("." + mSelectedItem)
      .show()
      .css("display", "inline-block");
  });
});
var componentProperties = [];
var dataSourcesInfo = [];
var userDefinedObjectsList = [];
var userMacrosList = [];
var allCompnentsComments = [];
//Change zoom level of drawflow workspace
function changeZoomLevel() {
  //editor.precanvas.style.left = 0 + 'px';
  //editor.precanvas.style.top = 0 + 'px';
  //editor.canvas_x = 0;
  //editor.canvas_y = 0;
  //
  editor.canvas_x = parseFloat(workspaceTranslateX.replace("px", ""));
  editor.canvas_y = parseFloat(workspaceTranslateY.replace("px", ""));
  if (parseFloat(workspaceZoomLevel) > 1) {
    var subs = (parseFloat(workspaceZoomLevel) - 1).toFixed(1);
    for (var i = 0; i < subs; i = i + 0.1) {
      editor.zoom_in();
    }
  } else if (parseFloat(workspaceZoomLevel) < 1) {
    var subs = (1 - parseFloat(workspaceZoomLevel)).toFixed(1);
    for (var i = 0; i < subs; i = i + 0.1) {
      editor.zoom_out();
    }
  } else {
    editor.zoom_reset();
  }
}
//Get component details of model
function getComponentDetails(isBackPressed, gotoLevel) {
  var mID = "";
  var mNodeName = "";
  //
  var levelNumber = "0";

  //
  editor.clearModuleSelected();
  //
  if (gotoLevel == null) {
    if (jQuery(".detailsLevel2").css("display") != "none") {
      levelNumber = "2";
    } else if (jQuery(".detailsLevel").css("display") != "none") {
      levelNumber = "1";
    }
    if (isBackPressed && levelNumber != "0")
      levelNumber = (parseInt(levelNumber) - 1).toString();
  } else {
    levelNumber = gotoLevel.toString();
  }
  if (levelNumber == "0") {
    jQuery(".detailsLevel").hide();
    jQuery(".detailsLevel2").hide();
    jQuery(
      ".Carbon_MitigationLevel1,.RefineryLevel,.realEstateLevel,.Green_HydrogenModel,.GeoThermalLevel"
    ).hide();
    //
    jQuery(
      ".Carbon_MitigationLevel2,.RefineryLevel,.FleetLevel,.realEstateLevel,.Green_HydrogenModel"
    ).hide();
    if (modelType == "Carbon_Mitigation")
      jQuery(".Carbon_MitigationLevel1").show();
    else if (modelType == "Green_Hydrogen")
      jQuery(".Green_HydrogenModel").show();
    getexportData(0);
    return;
  } else if (levelNumber == "1") {
    jQuery(".detailsLevel2").hide();
    //
    jQuery(".Carbon_MitigationLevel2").show();
    jQuery(
      ".Carbon_MitigationLevel1,.RefineryLevel,.realEstateLevel,.Green_HydrogenModel,.GeoThermalLevel"
    ).hide();
    mID = jQuery("#detailsLevelContainer").attr("currentNodeID");
    mNodeName = jQuery("#detailsLevelContainer").text();
  } else if (levelNumber == "2") {
    mID = jQuery("#Level2Container").attr("currentNodeID");
    mNodeName = jQuery("#Level2Container").text();
  }
  //
  if (mNodeName.toLowerCase().indexOf("refinery") != -1) {
    jQuery(
      ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.Green_HydrogenModel,.GeoThermalLevel"
    ).hide();
    jQuery(".RefineryLevel").show().css("display", "inline-block");
  } else if (mNodeName.toLowerCase().indexOf("fleet") != -1) {
    jQuery(
      ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.Green_HydrogenModel,.GeoThermalLevel"
    ).hide();
    jQuery(".FleetLevel").show().css("display", "inline-block");
  } else if (mNodeName.toLowerCase().indexOf("real") != -1) {
    jQuery(
      ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.Green_HydrogenModel,.GeoThermalLevel"
    ).hide();
    jQuery(".realEstateLevel").show().css("display", "inline-block");
  } else if (mNodeName.toLowerCase().indexOf("green") != -1) {
    jQuery(
      ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.realEstateLevel"
    ).hide();
    jQuery(".Green_HydrogenModel").show().css("display", "inline-block");
  } else if (mNodeName.toLowerCase().indexOf("geothermal") != -1) {
    jQuery(
      ".Carbon_MitigationLevel1,.Carbon_MitigationLevel2,.realEstateLevel,.Green_HydrogenModel"
    ).hide();
    jQuery(".GeoThermalLevel").show().css("display", "inline-block");
  }
  $.ajax({
    url:
      "/api/WebAPI_Design/getComponentsDetailsModel/" +
      subProjectID +
      "/" +
      mID +
      "/" +
      mNodeName +
      "/" +
      levelNumber,
    type: "GET",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      editor.zoom_reset();
      if (typeof response == "undefined") return;
      if (
        typeof response.exportJSON != "undefined" &&
        response.exportJSON != "" &&
        response.exportJSON != null
      )
        editor.import(JSON.parse(response.exportJSON));
      if (typeof response.workspaceZoomLevel != "undefined")
        workspaceZoomLevel = response.workspaceZoomLevel;
      else workspaceZoomLevel = "1";
      //
      if (typeof response.workspaceTranslateX != "undefined") {
        workspaceTranslateX = response.workspaceTranslateX;
        workspaceTranslateY = response.workspaceTranslateY;
      } else {
        workspaceTranslateX = "0";
        workspaceTranslateY = "0";
      }
      changeZoomLevel();
    },
    error: function (response) {},
  });
}
//Save component details and post data to web api
function saveComponentDetailsModel(isBackPressed, gotoLevel) {
  if (
    jQuery(".detailsLevel2").css("display") == "none" &&
    jQuery(".detailsLevel").css("display") == "none"
  ) {
    //if was in root level
    return;
  }
  var nodeName = jQuery("#detailsLevelContainer").text();
  var levelNumber = "1";
  if (jQuery(".detailsLevel2").css("display") != "none") {
    levelNumber = "2";
    nodeName = jQuery("#Level2Container").text();
  }
  var mID = "";
  if (jQuery(".detailsLevel2").css("display") != "none")
    mID = jQuery("#Level2Container").attr("currentNodeID");
  else if (jQuery(".detailsLevel").css("display") != "none")
    mID = jQuery("#detailsLevelContainer").attr("currentNodeID");
  //
  var componentDetail = {
    subProjectID: subProjectID,
    exportJSON: JSON.stringify(editor.export(), null, 4),
    nodeName: nodeName,
    nodeID: mID,
    levelNumber: levelNumber,
    workspaceZoomLevel: workspaceZoomLevel,
    workspaceTranslateX: editor.canvas_x.toString(),
    workspaceTranslateY: editor.canvas_y.toString(),
  };
  $.ajax({
    url: "/api/WebAPI_Design/saveComponentDetailsModel",
    type: "POST",
    data: JSON.stringify(componentDetail),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      jQuery("#btnSave").attr("disabled", false);
      getComponentDetails(isBackPressed, gotoLevel);
    },
    error: function (response) {},
  });
}

//Save all components and propertiese of them
function saveModel(statusType) {
  var mInfo = [];
  jQuery(".drawflow-node").each(function (Index, Val) {
    var priority = jQuery(this).find(".innerDiv").attr("priority");
    var mID = jQuery(this).attr("id");
    mID = mID.replace("node-", "");
    var mComponent = editor.getNodeFromId(mID);
    var nodePropertiese = [];
    var lat;
    var lng;
    //#region Properties
    var mProperties = {};
    if (typeof mComponent.data.length != "undefined") {
      mProperties = JSON.parse(mComponent.data);
    }
    for (var key in mProperties) {
      nodePropertiese.push({
        name: key,
        value: mProperties[key],
      });
      if (key == "location") {
        lat = mProperties[key].split(",")[0];
        lng = mProperties[key].split(",")[1];
      }
    }
    //#endregion Properties
    //#region Input Connections
    var inputConnections = [];
    if (
      mComponent.inputs.hasOwnProperty("input_1") ||
      mComponent.inputs.hasOwnProperty("input_2")
    ) {
      for (var key in mComponent.inputs) {
        jQuery(mComponent.inputs[key].connections).each(function (index, Val) {
          inputConnections.push({
            node: Val.node, //it means from component with this id to current component. distinguish source node
            source: Val.input,
            destination: key,
            nodeType: "input",
          });
        });
      }
    }
    //#endregion Input Connections
    //#region Output Connections
    var outputConnections = [];
    if (
      mComponent.outputs.hasOwnProperty("output_1") ||
      mComponent.outputs.hasOwnProperty("output_2")
    ) {
      for (var key in mComponent.outputs) {
        jQuery(mComponent.outputs[key].connections).each(function (index, Val) {
          outputConnections.push({
            node: Val.node, //Destincation component id
            source: key,
            destination: Val.output,
            nodeType: "output",
          });
        });
      }
    }
    //#endregion Output Connections
    mInfo.push({
      nodeID: mComponent.id.toString(),
      nodeName: mComponent.name,
      type: mComponent.name,
      positionTop: mComponent.pos_x.toString(),
      positionLeft: mComponent.pos_y.toString(),
      inputConnections: inputConnections,
      outputConnections: outputConnections,
      Propertiese: nodePropertiese,
      nodeHtmlContent: mComponent.html,
      latitude: lat,
      longitude: lng,
      subProjectID: subProjectID,
      priority: priority,
      inputConnectionsCount: jQuery("#node-" + mComponent.id)
        .find(".input")
        .length.toString(),
      outputConnectionsCount: jQuery("#node-" + mComponent.id)
        .find(".output")
        .length.toString(),
    });
  });
  jQuery("#btnSave").attr("disabled", "disabled");
  //
  $.ajax({
    url: "/api/WebAPI_Design/saveTemplateDesign",
    type: "POST",
    data: JSON.stringify(mInfo),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      saveSbProjectExportDataFlow(
        subProjectID,
        JSON.stringify(editor.export(), null, 4),
        statusType
      );
    },
    error: function (response) {
      saveSbProjectExportDataFlow(
        subProjectID,
        JSON.stringify(editor.export(), null, 4),
        statusType
      );
    },
    failure: function (response) {
      saveSbProjectExportDataFlow(
        subProjectID,
        JSON.stringify(editor.export(), null, 4),
        statusType
      );
    },
  });
  return mInfo;
}
//Sent json template and zoom level to web api
function saveSbProjectExportDataFlow(subProjectID, ExportData, statusType) {
  var mData = {
    subProjectID: subProjectID,
    exportJSON: ExportData,
    workspaceZoomLevel: workspaceZoomLevel,
    workspaceTranslateX: editor.canvas_x.toString(),
    workspaceTranslateY: editor.canvas_y.toString(),
  };
  $.ajax({
    url: "/api/WebAPI_Design/saveExportDataToSubProject",
    type: "POST",
    data: JSON.stringify(mData),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      if (statusType == 1) alertify.success("Model saved successfully");
      else {
        editor.clearModuleSelected();
        getComponentDetails(false, null);
      }
      jQuery("#btnSave").attr("disabled", false);
    },
    error: function (response) {
      if (statusType == 1) alertify.success("Model saved successfully");
      else {
        getComponentDetails(false, null);
      }
      jQuery("#btnSave").attr("disabled", false);
    },
  });
}
//Get json template for current sub project
function getexportData(levelNumber) {
  $.ajax({
    url: "/api/WebAPI_Design/getExportData/" + subProjectID,
    type: "GET",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      editor.zoom_reset();
      if (
        typeof response.exportJSON != "undefined" &&
        response.exportJSON != "" &&
        response.exportJSON != null
      )
        editor.import(JSON.parse(response.exportJSON));
      if (
        typeof response.workspaceZoomLevel != "undefined" &&
        response.workspaceZoomLevel != null
      )
        workspaceZoomLevel = response.workspaceZoomLevel;
      else workspaceZoomLevel = "1";
      //
      if (typeof response.workspaceTranslateX != "undefined") {
        workspaceTranslateX = response.workspaceTranslateX;
        workspaceTranslateY = response.workspaceTranslateY;
      } else {
        workspaceTranslateX = "0";
        workspaceTranslateY = "0";
      }
      changeZoomLevel();
      //
      // if (response.workFlowStatus == "Approved") jQuery("#btnSave").hide();
      getComponentComments();
    },
    error: function (response) {},
    failure: function (response) {},
  });
}
//Show input/output nodes in modal
function getConnectionsForModal() {
  var inputCount = $selectedNodeForContext.find(".input").length;
  var outputCount = $selectedNodeForContext.find(".output").length;
  jQuery("#inputList,#outputList").empty();
  for (var i = 0; i < inputCount; i++) {
    jQuery("#inputList").append(
      '<li class="show" nodeID="input_' +
        (i + 1).toString() +
        '"><span>input - ' +
        (i + 1) +
        '<span><a  nodeID="input_' +
        (i + 1).toString() +
        '" href="#" class="inputItem deleteFromInputOutputNodes" style="float:right"><i class="far fa-trash-alt"></i></a><li>'
    );
  }

  for (var i = 0; i < outputCount; i++) {
    jQuery("#outputList").append(
      '<li class="show" nodeID="output_' +
        (i + 1).toString() +
        '"><span>output - ' +
        (i + 1) +
        '<span><a nodeID="output_' +
        (i + 1).toString() +
        '" href="#" class="outputItem deleteFromInputOutputNodes" style="float:right"><i class="far fa-trash-alt"></i></a><li>'
    );
  }
}
//Add new node connection to component(input/output)
function addNewConnectin(nodeType) {
  var mID = $selectedNodeForContext.attr("id");
  mID = mID.replace("node-", "");
  //
  if (nodeType == "input") editor.addNodeInput(mID);
  else editor.addNodeOutput(mID);
  getConnectionsForModal();
}
//Get comments for components from web api
function getComponentComments() {
  $.ajax({
    url: "/api/WebAPI_ComponentComments/getComponentComments/" + subProjectID,
    type: "GET",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      allCompnentsComments = response;
      jQuery(response).each(function (Index, Val) {
        jQuery("#" + Val.nodeID).append(
          '<div class="componentComment" nodeID="' +
            Val.nodeID +
            '" compnentID="' +
            Val.id +
            '" title="Click to show comments"><i class="fas fa-comments"></i></div>'
        );
      });
    },
    error: function (response) {},
    failure: function (response) {},
  });
}
//Send comment for specific component to web api
function saveComponentComment() {
  var mInfo = {
    subProjectID: subProjectID,
    nodeID: $selectedNodeForContext.attr("id"),
    componentName: $selectedNodeForContext.attr("class").split(" ")[1],
    commentText: jQuery("#txtComponentComment").val(),
  };
  $.ajax({
    url: "/api/WebAPI_ComponentComments/saveComments",
    type: "POST",
    data: JSON.stringify(mInfo),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      jQuery("#ComponentCommentModal").modal("hide");
      alertify.log("Comment saved.");
      getComponentComments();
    },
    error: function (response) {
      jQuery("#ComponentCommentModal").modal("hide");
      alertify.log("Comment saved.");
      getComponentComments();
    },
    failure: function (response) {},
  });
}
//Approve model, when click on aprove button
function doApproveSharedProject() {
  alertify.confirm("Are you sure to approve?", function (e) {
    if (e) {
      $.ajax({
        url: "/api/WebAPI_Projects/approveSharedProject/" + subProjectID,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
          // jQuery("#btnApproveSharedProject,#btnSave").hide();
          alertify.log("Project approved.");
        },
        error: function (response) {
          // jQuery("#btnApproveSharedProject,#btnSave").hide();
          alertify.log("Project approved.");
        },
        failure: function (response) {},
      });
    }
  });
}
//Get all permissions for this sub project from web api
function checkPermissionSharedProject() {
  $.ajax({
    url: "/api/WebAPI_Projects/checkPermissionSharedProject/" + subProjectID,
    type: "GET",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      if (response.length > 0) {
        if (
          response[0].permission == "Approve" &&
          response[0].sharedProjectStatus != "Approved"
        )
          jQuery("#btnApproveSharedProject").show();
        else if (
          response[0].permission == "Approve" &&
          response[0].sharedProjectStatus == "Approved"
        )
          // jQuery("#btnApproveSharedProject,#btnSave").hide();
          jQuery("#btnApproveSharedProject").hide();
        else if (response[0].permission == "View") jQuery("#btnSave").hide();
      }
    },
    error: function (response) {},
    failure: function (response) {},
  });
}
//Back to top level and back to project page
function backtoProject() {
  var folder = getParameterByName("folder");
  window.location.href = "/Home?project=" + folder;
}
//When click on cancel macro in macro panel
function doCancelMacro() {
  jQuery("#MacroList").hide();
  jQuery("#list").empty();
  jQuery(".drawflow-node").find(".checkASMacro").remove();
}
//Get all macros for current user from web api
function getUserMacros() {
  $.ajax({
    url: "/api/WebAPI_Macros/getUserMacros",
    type: "GET",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      jQuery("#userMacrosContainer").empty();
      userMacrosList = response;
      if (response.length > 0) {
        jQuery(response).each(function (Index, Val) {
          var htmlCode =
            '<div mID="' +
            Val.id +
            '" customName="Macro_' +
            Val.name +
            '" mID="' +
            Val.id +
            '" class="drag-drawflow userMacroIcons" draggable="true" ondragstart="drag(event)" data-node="' +
            Val.name +
            '">';
          htmlCode +=
            '<img src="../Images/Macro.png" title="' +
            Val.name +
            '" data-node="Macro_' +
            Val.name +
            '"/>';
          //htmlCode += '<div mID="' + Val.id + '" draggable="false" class="deleteUserMacro"><a href="#">&#215;</a></div>';
          htmlCode +=
            '<div draggable="false" class="macroTitle">' + Val.name + "</div>";
          htmlCode += "</div>";
          jQuery("#userMacrosContainer").append(htmlCode);
        });
      }
    },
    error: function (response) {},
    failure: function (response) {},
  });
}
//Send macro data to web api
function saveMacro() {
  if (jQuery("#txtMacroName").val() == "") {
    jQuery("#txtMacroName").focus();
    alertify.error("Name is mandatory.");
    return;
  }
  var mInfo = [];
  jQuery("#list")
    .find(".show")
    .each(function (Index, Val) {
      var priority = "0";
      var mID = jQuery(this).attr("nodeid");
      mID = mID.replace("node-", "");
      var mComponent = editor.getNodeFromId(mID);
      var nodePropertiese = [];
      var lat;
      var lng;
      //#region Properties
      var mProperties = {};
      if (typeof mComponent.data.length != "undefined") {
        mProperties = JSON.parse(mComponent.data);
      }
      for (var key in mProperties) {
        nodePropertiese.push({
          name: key,
          value: mProperties[key],
        });
        if (key == "location") {
          lat = mProperties[key].split(",")[0];
          lng = mProperties[key].split(",")[1];
        }
      }
      //#endregion Properties
      //#region Input Connections
      var inputConnections = [];
      if (
        mComponent.inputs.hasOwnProperty("input_1") ||
        mComponent.inputs.hasOwnProperty("input_2")
      ) {
        for (var key in mComponent.inputs) {
          jQuery(mComponent.inputs[key].connections).each(function (
            index,
            Val
          ) {
            inputConnections.push({
              node: (parseFloat(Val.node) + 1000).toString(), //it means from component with this id to current component. distinguish source node
              source: Val.input,
              destination: key,
              nodeType: "input",
            });
          });
        }
      }
      //#endregion Input Connections
      //#region Output Connections
      var outputConnections = [];
      if (
        mComponent.outputs.hasOwnProperty("output_1") ||
        mComponent.outputs.hasOwnProperty("output_2")
      ) {
        for (var key in mComponent.outputs) {
          jQuery(mComponent.outputs[key].connections).each(function (
            index,
            Val
          ) {
            outputConnections.push({
              node: (parseFloat(Val.node) + 1000).toString(), //Destincation component id
              source: key,
              destination: Val.output,
              nodeType: "output",
            });
          });
        }
      }
      //#endregion Output Connections
      mInfo.push({
        nodeID: (parseFloat(mComponent.id) + 1000).toString(),
        nodeName: mComponent.name,
        type: mComponent.name,
        positionTop: mComponent.pos_x.toString(),
        positionLeft: mComponent.pos_y.toString(),
        inputConnections: inputConnections,
        outputConnections: outputConnections,
        Propertiese: nodePropertiese,
        nodeHtmlContent: mComponent.html,
        latitude: lat,
        longitude: lng,
        subProjectID: subProjectID,
        priority: priority,
        inputConnectionsCount: jQuery("#node-" + mComponent.id)
          .find(".input")
          .length.toString(),
        outputConnectionsCount: jQuery("#node-" + mComponent.id)
          .find(".output")
          .length.toString(),
      });
    });
  var macroInfo = {
    name: jQuery("#txtMacroName").val(),
    components: mInfo,
  };
  jQuery("#btnSaveMacro").attr("disabled", "disabled");
  //
  $.ajax({
    url: "/api/WebAPI_Macros/saveMacro",
    type: "POST",
    data: JSON.stringify(macroInfo),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      getUserMacros();
      jQuery("#btnSaveMacro").removeAttr("disabled");
      alertify.success("Macro saved succeed.");
      jQuery("#txtMacroName").val("");
      doCancelMacro();
      jQuery("#MacroModal").modal("hide");
    },
    error: function (response) {
      getUserMacros();
      jQuery("#btnSaveMacro").removeAttr("disabled");
      alertify.success("Macro saved succeed.");
      jQuery("#txtMacroName").val("");
      doCancelMacro();
      jQuery("#MacroModal").modal("hide");
    },
    failure: function (response) {},
  });
}
//Add component to macro list(macro panel in right side of the page)
//When CTRL button is pressed
function addToMacroList(id) {
  jQuery("#MacroList").show();
  var mComponent = editor.getNodeFromId(id);
  if (jQuery("#list").find('li[nodeID="' + mComponent.id + '"]').length > 0) {
    return;
  }
  var mProperties = {};
  if (typeof mComponent.data.length != "undefined") {
    mProperties = JSON.parse(mComponent.data);
  }
  var componentName = "";
  for (var key in mProperties) {
    if (key == "Name") {
      componentName = mProperties[key];
      break;
    }
  }
  jQuery("#node-" + mComponent.id).append(
    '<i class="far fa-check-circle checkASMacro"></i>'
  );
  jQuery("#list").append(
    '<li nodeID="' +
      mComponent.id +
      '"><span>' +
      componentName +
      '<span><a href="#" class="deleteFromMacroList" style="float:right"><i class="far fa-trash-alt"></i></a><li>'
  );
  setTimeout(function () {
    jQuery("#list")
      .find('li[nodeID="' + mComponent.id + '"]')
      .addClass("show");
  }, 10);
}
//Get all user defined object from web api
function getUserDefinedObjects() {
  $.ajax({
    url: "/api/WebAPI_UserDefinedObjects/getUserDefinedObject",
    type: "GET",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      jQuery("#userDeefinedObjectContainer").empty();
      userDefinedObjectsList = response;
      if (response.length > 0) {
        jQuery(response).each(function (Index, Val) {
          var htmlCode =
            '<div mID="' +
            Val.id +
            '" customName="userObject_' +
            Val.name +
            '" class="drag-drawflow userDefinedObjectIcons" draggable="true" ondragstart="drag(event)" data-node="turbine">';
          htmlCode +=
            '<img src="https://dsiderinc-bucket.s3.amazonaws.com/' +
            Val.imagePath +
            '" title="' +
            Val.name +
            '" data-node="userObject_' +
            Val.name +
            '"/>';
          //htmlCode += '<div mID="' + Val.id + '" draggable="false" class="deleteUserDefinedObject"><a href="#">&#215;</a></div>';
          htmlCode += "</div>";
          jQuery("#userDeefinedObjectContainer").append(htmlCode);
        });
      }
    },
    error: function (response) {},
    failure: function (response) {},
  });
}
//Send user defined object data to web api
function saveUserDefinedObject() {
  if (jQuery("#txtUserDefinedObjectName").val() == "") {
    jQuery("#txtUserDefinedObjectName").focus();
    alertify.error("Name is mandatory.");
    return;
  }
  if (jQuery("#txtUserDefinedObjectUnitName").val() == "") {
    jQuery("#txtUserDefinedObjectUnitName").focus();
    alertify.error("Unit name is mandatory.");
    return;
  }
  if (jQuery("#txtUserDefinedObjectDefaultValue").val() == "") {
    jQuery("#txtUserDefinedObjectDefaultValue").focus();
    alertify.error("Default value is mandatory.");
    return;
  }
  if (jQuery("#txtUserDefinedObjectInput").val() == "") {
    jQuery("#txtUserDefinedObjectInput").focus();
    alertify.error("Input count is mandatory.");
    return;
  }
  if (jQuery("#txtUserDefinedObjectOutput").val() == "") {
    jQuery("#txtUserDefinedObjectOutput").focus();
    alertify.error("Output count is mandatory.");
    return;
  }
  //if (jQuery('#txtUserDefinedObjectScript').val() == '') {
  //    jQuery('#txtUserDefinedObjectScript').focus();
  //    alertify.error('Script is mandatory.');
  //    return;
  //}
  var fileUpload = jQuery("#filesUserObjectIcon").get(0);
  var files = fileUpload.files;
  if (files.length == 0) {
    alertify.error("Select icon file.");
    return;
  }
  jQuery("#btnSaveObject").attr("disabled", "disabled");
  var mData = {
    name: jQuery("#txtUserDefinedObjectName").val(),
    inputCount: jQuery("#txtUserDefinedObjectInput").val(),
    OutputCount: jQuery("#txtUserDefinedObjectOutput").val(),
    uniName: "", //jQuery('#txtUserDefinedObjectUnitName').val(),
    objectType: "User Deined Object",
    defaultValue: jQuery("#txtUserDefinedObjectDefaultValue").val(),
    script: jQuery("#txtUserDefinedObjectScript").val(),
  };
  $.ajax({
    url: "/api/WebAPI_UserDefinedObjects/saveUserDefinedObject",
    type: "POST",
    data: JSON.stringify(mData),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      Upload_UserDefinedObject(response);
    },
    error: function (response) {
      Upload_UserDefinedObject(response.responseText);
    },
    failure: function (response) {
      Upload_UserDefinedObject(response);
    },
  });
}
//Upload user defined object icon
function Upload_UserDefinedObject(ID) {
  var fileUpload = jQuery("#filesUserObjectIcon").get(0);
  var files = fileUpload.files;
  var data = new FormData();
  data.append(files[0].name, files[0]);
  createCookie("ObjecID", ID);
  $.ajax({
    type: "POST",
    url: "/api/WebAPI_Uploader/Upload_UserDefinedObject",
    contentType: false,
    processData: false,
    data: data,
    async: false,
    beforeSend: function () {
      $("#divloader").show();
    },
    success: function (message) {
      alertify.success("User Defined Object saved succeed.");
      getUserDefinedObjects();
      jQuery("#btnSaveObject").removeAttr("disabled");
      jQuery("#addNewUserDefinedObjectModal").modal("hide");
      jQuery("#txtUserDefinedObjectInput,#txtUserDefinedObjectOutput").val(0);
      jQuery(
        "#txtUserDefinedObjectName,#txtUserDefinedObjectDefaultValue,#txtUserDefinedObjectScript"
      ).val("");
    },
    error: function (x, e) {
      alert("Error!");
    },
    complete: function () {
      $("#divloader").hide();
      jQuery("#addNewUserDefinedObjectModal").modal("hide");
    },
  });
}
//Reveal add new user Defined object
function addNewUserDefinedObject() {
  jQuery("#addNewUserDefinedObjectModal").modal("show");
}

//Get all propertiese from web api
function getComponentsProperties() {
  $.ajax({
    url: "/api/WebAPI_Design/getComponentsProperties",
    type: "GET",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      componentProperties = response;
    },
    error: function (response) {},
    failure: function (response) {},
  });
}
//Get sub project details
function getSubProjectDetails(subProjectID) {
  $.ajax({
    url: "/api/WebAPI_Design/getSubProjectDetails/" + subProjectID,
    type: "GET",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      if (response[0].workFlowStatus == "Approved") jQuery("#btnSave").hide();
      //if (typeof response[0].workspaceZoomLevel != 'undefined')
      //    workspaceZoomLevel = response[0].workspaceZoomLevel;
      //else
      //    workspaceZoomLevel = '1';
      //changeZoomLevel();
    },
    error: function (response) {},
    failure: function (response) {},
  });
}
//Get all datasources to show in DataSources modal
function getDataSources() {
  $.ajax({
    url: "/api/WebAPI_DataSource/selectDataSources/" + subProjectID,
    type: "POST",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      //jQuery('#trDataSource').show();
      //jQuery('.DataSource').html('<option value=""></option>');
      jQuery(".dataSourcesRow").remove();
      dataSourcesInfo = response;
      jQuery(response).each(function (index, val) {
        //jQuery('.DataSource').append('<option value="' + val.id + '">' + val.name + '</option>');
        jQuery("#tblDataSources").append(
          '<tr class="dataSourcesRow"><td style="color:black;">' +
            val.name +
            '</td><td><i mID="' +
            val.id +
            '" class="fa fa-minus-circle removeDataSource" style="color:red; cursor:pointer;" title="Remove Data Source"></i></td></tr>'
        );
      });
    },
    error: function (response) {},
    failure: function (response) {},
  });
}
//Put value of each property by selcted component
function showPropertyValuesByNodeID(nodeID) {
  jQuery(".dynamicProperties").remove();
  jQuery(".propertyItem").val("");
  var componentType = $selectedNodeForContext[0].className.split(" ")[1];
  if ($selectedNodeForContext.hasClass("battery")) componentType = "battery";
  else if (
    $selectedNodeForContext.hasClass("turbine") ||
    $selectedNodeForContext.hasClass("Solar")
  )
    componentType = "Renewable Power";
  else if ($selectedNodeForContext.hasClass("electrolyzer"))
    componentType = "Electrolyser";
  else if ($selectedNodeForContext.hasClass("StorageH2"))
    componentType = "Hydrogen Storage";
  else if ($selectedNodeForContext.hasClass("Mobility"))
    componentType = "Hydrogen Demand Mobility";
  else if ($selectedNodeForContext.hasClass("industriese"))
    componentType = "Hydrogen Demand Refinery";
  else if ($selectedNodeForContext.hasClass("Injection"))
    componentType = "Injection field";
  else if ($selectedNodeForContext.hasClass("Gas"))
    componentType = "Gas Field 1";
  else if ($selectedNodeForContext.hasClass("Wells")) componentType = "Wells 1";
  else if ($selectedNodeForContext.hasClass("Reservoir"))
    componentType = "Co2 Reservoir";

  //
  var customProperties = componentProperties.filter(function (el) {
    return el.component === componentType && el.paramType == "input";
  });
  if (customProperties != null && customProperties.length > 0)
    jQuery(customProperties).each(function (Index, Val) {
      var htmlCode =
        '<tr class="dynamicProperties"><td class="font-weight-bold">' +
        Val.propertyName +
        " (" +
        Val.unit +
        ")</td>";
      htmlCode += '<td title="' + Val.description + '">';
      if (Val.hasDatasource == "false")
        htmlCode +=
          '<input title="' +
          Val.description +
          '" type="text" style="width:75%;display:inline-block;" class="form-control propertyItem" value="' +
          Val.propertyValue +
          '" property="' +
          Val.formulaTitle +
          '" />';
      else {
        htmlCode +=
          '<input title="' +
          Val.description +
          '" type="text" class="form-control propertyItem" style="width:75%;display:inline-block; margin-right:5px;font-size:10px; padding:5px;" value="' +
          Val.propertyValue +
          '" property="' +
          Val.formulaTitle +
          '" /> ';
        htmlCode +=
          '<input type="button" class="btn btn-primary btnBrowseDataSource" property="' +
          Val.formulaTitle +
          '" style="display: inline-block;width: 10%;text-align: center;padding-left: 10px;background: #8080805c;" value="..."/>';
      }
      htmlCode += "</td></tr> ";
      jQuery("#propertyTable").append(htmlCode);
    });
  //
  var curerrentComponentID = nodeID;
  var mID = curerrentComponentID.replace("node-", "");
  var infoOfCurrentNode = editor.getNodeFromId(mID);
  var mProperties = {};
  if (typeof infoOfCurrentNode.data.length != "undefined") {
    mProperties = JSON.parse(infoOfCurrentNode.data);
    for (var key in mProperties) {
      jQuery('.propertyItem[property="' + key + '"]').val(mProperties[key]);
    }
  }
  //
  var outParam = componentProperties.filter(function (el) {
    return el.component === componentType && el.paramType == "output";
  });
  if (outParam != null && outParam.length > 0)
    jQuery(outParam).each(function (Index, Val) {
      var htmlCode =
        '<tr title="' +
        Val.description +
        '"class="dynamicProperties"><td class="font-weight-bold">' +
        Val.propertyName +
        " (" +
        Val.unit +
        ")</td>";
      htmlCode += "</tr> ";
      jQuery("#properyTableOutput").append(htmlCode);
    });
}
//Hide and close context menu
function closeContextMenu() {
  if (typeof $selectedNodeForContext != "undefined")
    $selectedNodeForContext.removeClass("active");
  var ul = $("#navs"),
    li = $("#navs li");
  li.removeAttr("style");
  jQuery("#navs li").hide();
}
//Upload datasource csv file
function UploadFile() {
  var fileUpload = $("#files").get(0);
  var files = fileUpload.files;
  if (jQuery("#txtDataSourceName").val() == "") {
    alertify.error("DataSource name is required");
    return;
  }
  if (files.length == 0) {
    alertify.error("Select your files");
    return;
  }
  var data = new FormData();
  data.append(files[0].name, files[0]);
  createCookie(
    "DataSourceName",
    jQuery("#txtDataSourceName").val().replace(/\ /g, "#@")
  );
  createCookie(
    "DataSourceDescription",
    jQuery("#txtDataSourceDescription").val().replace(/\ /g, "#@")
  );
  createCookie("subProjectID", subProjectID);
  $.ajax({
    type: "POST",
    url: "/api/WebAPI_Uploader/Upload_File",
    contentType: false,
    processData: false,
    data: data,
    async: false,
    beforeSend: function () {
      $("#divloader").show();
    },
    success: function (message) {
      if (message == "Error Name") {
        alertify.error("Data Source name is taken before in this project.");
        return;
      }
      alertify.success("File uploaded. Data Source created.");
      getDataSources();
      jQuery("#dataSourceModal").modal("hide");
    },
    error: function (x, e) {
      alert("Error!");
    },
    complete: function () {
      $("#divloader").hide();
      jQuery("#dataSourceModal").modal("hide");
    },
  });
}
//Get file name of selected file
function getFileName() {
  if (jQuery("#txtDataSourceName").val() == "") {
    var file = jQuery("#files")[0].files[0];
    if (file) {
      jQuery("#txtDataSourceName,#txtDataSourceDescription").val(
        file.name.replace(".csv", "")
      );
    }
  }
}

//Import json file to drawflow model
function importJSON() {
  var file = document.getElementById("jSONFiles").files[0];
  if (file) {
    var reader = new FileReader();
    reader.readAsText(file, "UTF-8");
    reader.onload = function (evt) {
      var mJOSN = JSON.parse(evt.target.result);
      $.ajax({
        url: "/api/WebAPI_Design/createComponentsProperties",
        type: "POST",
        data: JSON.stringify(mJOSN),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {},
        error: function (response) {},
      });
      editor.import(mJOSN);
      alertify.success("JSON file imported.");
    };
    reader.onerror = function (evt) {
      //document.getElementById("fileContents").innerHTML = "error reading file";
    };
  } else alertify.error("Select your file");
}
//#region DraFlow

editor.reroute = true;
editor.reroute_fix_curvature = true;
editor.force_first_input = false;

editor.start();
//To show context menu of component
editor.on("contextmenu", function (clickInfo) {
  var clientX = clickInfo.clientX;
  var clientY = clickInfo.clientY;
  jQuery("#navs").css({
    top: clientY + clickInfo.offsetY + "px",
    left: clientX + "px",
  });
  //console.log("Node created " + id);
  //var mComponent = editor.getNodeFromId(id);
});
//When component has been deleted
editor.on("nodeRemoved", function (id) {
  closeContextMenu();
});
//When click on component
editor.on("nodeSelected", function (id) {
  setTimeout(function () {
    $selectedNodeForContext = jQuery("#node-" + id);
    //getDataSources();
    showPropertyValuesByNodeID(id);
  }, 100);
  //
  if (cntrlIsPressed) addToMacroList(id);
  //
  jQuery(".connection").find("path").css("stroke", "#4ea9ff");
  jQuery(".node_in_node-" + id)
    .find("path")
    .css("stroke", "#4a8b78");
  jQuery(".node_out_node-" + id)
    .find("path")
    .css("stroke", "#4a8b78");
});
//When node moved in components
editor.on("nodeMoved", function (id) {
  jQuery(".connection").find("path").css("stroke", "#4ea9ff");
  jQuery(".node_in_node-" + id)
    .find("path")
    .css("stroke", "#4a8b78");
  jQuery(".node_out_node-" + id)
    .find("path")
    .css("stroke", "#4a8b78");
  //}, 100);
});
editor.on("zoom", function (zoom) {
  workspaceZoomLevel = zoom.toString();
});

/* DRAG EVENT */
/* Mouse and Touch Actions */
var elements = document.getElementsByClassName("drag-drawflow");
for (var i = 0; i < elements.length; i++) {
  elements[i].addEventListener("touchend", drop, false);
  elements[i].addEventListener("touchmove", positionMobile, false);
  elements[i].addEventListener("touchstart", drag, false);
}
var mobile_item_selec = "";
var mobile_last_move = null;
function positionMobile(ev) {
  mobile_last_move = ev;
}
function allowDrop(ev) {
  ev.preventDefault();
}
function drag(ev) {
  if (ev.type === "touchstart") {
    mobile_item_selec = ev.target
      .closest(".drag-drawflow")
      .getAttribute("data-node");
  } else {
    ev.dataTransfer.setData("node", ev.target.getAttribute("data-node"));
  }
}
function drop(ev) {
  if (ev.type === "touchend") {
    var parentdrawflow = document
      .elementFromPoint(
        mobile_last_move.touches[0].clientX,
        mobile_last_move.touches[0].clientY
      )
      .closest("#drawflow");
    if (parentdrawflow != null) {
      addNodeToDrawFlow(
        mobile_item_selec,
        mobile_last_move.touches[0].clientX,
        mobile_last_move.touches[0].clientY
      );
    }
    mobile_item_selec = "";
  } else {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("node");
    addNodeToDrawFlow(data, ev.clientX, ev.clientY);
  }
}
function createPostiFixForComponent(componentType) {
  var postfix = jQuery("." + componentType).length.toString();
  if (postfix == "0") postfix = "";
  else postfix = "-" + postfix;
  return postfix;
}
//When drag and drop new component from left panel to drawflow work space
function addNodeToDrawFlow(name, pos_x, pos_y) {
  if (editor.editor_mode === "fixed") {
    return false;
  }
  pos_x =
    pos_x *
      (editor.precanvas.clientWidth /
        (editor.precanvas.clientWidth * editor.zoom)) -
    editor.precanvas.getBoundingClientRect().x *
      (editor.precanvas.clientWidth /
        (editor.precanvas.clientWidth * editor.zoom));
  pos_y =
    pos_y *
      (editor.precanvas.clientHeight /
        (editor.precanvas.clientHeight * editor.zoom)) -
    editor.precanvas.getBoundingClientRect().y *
      (editor.precanvas.clientHeight /
        (editor.precanvas.clientHeight * editor.zoom));
  var postfix = createPostiFixForComponent(name);
  switch (name) {
    case "Aggregator":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Aggregator.png" class="iDesign" /><br> <span class="componentTitle">Aggregator` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Aggregator",
        1,
        1,
        pos_x,
        pos_y,
        "Aggregator",
        '{"Name":"Aggregator' + postfix + '"}',
        mCode
      );
      break;
    //GeoThermal Level
    case "Desalinization":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/GeoThermal/Desalinization.png" class="iDesign" /><br> <span class="componentTitle">Desalinization` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Desalinization",
        1,
        1,
        pos_x,
        pos_y,
        "Desalinization",
        '{"Name":"Desalinization' + postfix + '"}',
        mCode
      );
      break;
    case "Aquaculture":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/GeoThermal/Aquaculture.png" class="iDesign" /><br> <span class="componentTitle">Aquaculture` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Aquaculture",
        1,
        1,
        pos_x,
        pos_y,
        "Aquaculture",
        '{"Name":"Aquaculture' + postfix + '"}',
        mCode
      );
      break;
    case "Chiller":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/GeoThermal/Chiller.png" class="iDesign" /><br> <span class="componentTitle">Absorption Chiller` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Chiller",
        1,
        1,
        pos_x,
        pos_y,
        "Chiller",
        '{"Name":"Absorption Chiller' + postfix + '"}',
        mCode
      );
      break;
    case "GASTurbine":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/GeoThermal/GASTurbine.png" class="iDesign" /><br> <span class="componentTitle">GAS Turbine` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "GASTurbine",
        1,
        1,
        pos_x,
        pos_y,
        "GASTurbine",
        '{"Name":"GAS Turbine' + postfix + '"}',
        mCode
      );
      break;
    case "GreenHouse":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/GeoThermal/GreenHouse.png" class="iDesign" /><br> <span class="componentTitle">GreenHouse` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "GreenHouse",
        1,
        1,
        pos_x,
        pos_y,
        "GreenHouse",
        '{"Name":"GreenHouse' + postfix + '"}',
        mCode
      );
      break;
    case "OrcPowerPlant":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/GeoThermal/OrcPowerPlant.png" class="iDesign" /><br> <span class="componentTitle">Orc Power Plant` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "OrcPowerPlant",
        1,
        1,
        pos_x,
        pos_y,
        "OrcPowerPlant",
        '{"Name":"Orc Power Plant' + postfix + '"}',
        mCode
      );
      break;
    case "WasteHeat":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/GeoThermal/WasteHeat.png" class="iDesign" /><br> <span class="componentTitle">Waste Heat Recovery` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "WasteHeat",
        1,
        1,
        pos_x,
        pos_y,
        "WasteHeat",
        '{"Name":"Waste Heat' + postfix + '"}',
        mCode
      );
      break;
    //real Estate
    case "Office":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/RealEstate/House.png" class="iDesign" /><br> <span class="componentTitle">Office` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Office",
        1,
        1,
        pos_x,
        pos_y,
        "Office",
        '{"Name":"Office' + postfix + '"}',
        mCode
      );
      break;
    case "Tower":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/RealEstate/Tower.png" class="iDesign" /><br> <span class="componentTitle">Tower` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Tower",
        1,
        1,
        pos_x,
        pos_y,
        "Tower",
        '{"Name":"Tower' + postfix + '"}',
        mCode
      );
      break;
    //Fleet
    case "EvTruck":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Truck/EvTruck.png" class="iDesign" /><br> <span class="componentTitle">EV Truck` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "EvTruck",
        1,
        1,
        pos_x,
        pos_y,
        "EvTruck",
        '{"Name":"EV Truck' + postfix + '"}',
        mCode
      );
      break;
    case "FuelTrack":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Truck/FuelTrack.png" class="iDesign" /><br> <span class="componentTitle">Fuel Truck` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "FuelTrack",
        1,
        1,
        pos_x,
        pos_y,
        "FuelTrack",
        '{"Name":"Fuel Truck' + postfix + '"}',
        mCode
      );
      break;
    case "H2Truck":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Truck/H2Truck.png" class="iDesign" /><br> <span class="componentTitle">H2 Truck` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "H2Truck",
        1,
        1,
        pos_x,
        pos_y,
        "H2Truck",
        '{"Name":"H2 Truck' + postfix + '"}',
        mCode
      );
      break;
    //Refinery
    case "Boiler":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/Boiler.png" class="iDesign" /><br> <span class="componentTitle">Boiler` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Boiler",
        1,
        1,
        pos_x,
        pos_y,
        "Boiler",
        '{"Name":"Boiler' + postfix + '"}',
        mCode
      );
      break;
    case "CatalyticCracker":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/CatalyticCracker.png" class="iDesign" /><br> <span class="componentTitle">Catalytic Cracker` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "CatalyticCracker",
        1,
        1,
        pos_x,
        pos_y,
        "CatalyticCracker",
        '{"Name":"Catalytic Cracker' + postfix + '"}',
        mCode
      );
      break;
    case "CHPGeneration":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/CHPGeneration.png" class="iDesign" /><br> <span class="componentTitle">CHP Generation` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "CHPGeneration",
        1,
        1,
        pos_x,
        pos_y,
        "CHPGeneration",
        '{"Name":"CHP Generation' + postfix + '"}',
        mCode
      );
      break;
    case "Compressor":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/Compressor.png" class="iDesign" /><br> <span class="componentTitle">Compressor` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Compressor",
        1,
        1,
        pos_x,
        pos_y,
        "Compressor",
        '{"Name":"Compressor' + postfix + '"}',
        mCode
      );
      break;
    case "Cooling":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/Cooling.png" class="iDesign" /><br> <span class="componentTitle">Cooling` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Cooling",
        1,
        1,
        pos_x,
        pos_y,
        "Cooling",
        '{"Name":"Compressor' + postfix + '"}',
        mCode
      );
      break;
    case "DistillationFurnace":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/DistillationFurnace.png" class="iDesign" /><br> <span class="componentTitle">Distillation Furnace` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "DistillationFurnace",
        1,
        1,
        pos_x,
        pos_y,
        "DistillationFurnace",
        '{"Name":"Distillation Furnace' + postfix + '"}',
        mCode
      );
      break;
    case "Fan":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/Fan.png" class="iDesign" /><br> <span class="componentTitle">Fan` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Fan",
        1,
        1,
        pos_x,
        pos_y,
        "Fan",
        '{"Name":"Fan' + postfix + '"}',
        mCode
      );
      break;
    case "HydroTreater":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/HydroTreater.png" class="iDesign" /><br> <span class="componentTitle">HydroTreater` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "HydroTreater",
        1,
        1,
        pos_x,
        pos_y,
        "HydroTreater",
        '{"Name":"HydroTreater' + postfix + '"}',
        mCode
      );
      break;
    case "Isomerization":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/Isomerization.png" class="iDesign" /><br> <span class="componentTitle">Isomerization` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Isomerization",
        1,
        1,
        pos_x,
        pos_y,
        "Isomerization",
        '{"Name":"Isomerization' + postfix + '"}',
        mCode
      );
      break;
    case "MeroxTreater":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/MeroxTreater.png" class="iDesign" /><br> <span class="componentTitle">MeroxTreater` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "MeroxTreater",
        1,
        1,
        pos_x,
        pos_y,
        "MeroxTreater",
        '{"Name":"MeroxTreater' + postfix + '"}',
        mCode
      );
      break;
    case "Pump":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/Pump.png" class="iDesign" /><br> <span class="componentTitle">Pump` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Pump",
        1,
        1,
        pos_x,
        pos_y,
        "Pump",
        '{"Name":"Pump' + postfix + '"}',
        mCode
      );
      break;
    case "Machine":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/machine.png" class="iDesign" /><br> <span class="componentTitle">Machine` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Machine",
        1,
        1,
        pos_x,
        pos_y,
        "Machine",
        '{"Name":"Machine' + postfix + '"}',
        mCode
      );
      break;
    case "Process":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Refinery/process.png" class="iDesign" /><br> <span class="componentTitle">Process` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Process",
        1,
        1,
        pos_x,
        pos_y,
        "Process",
        '{"Name":"Process' + postfix + '"}',
        mCode
      );
      break;
    //
    case "Fleet":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Level1/fleet.png" class="iDesign" /><br> <span class="componentTitle">Fleet` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Fleet",
        0,
        1,
        pos_x,
        pos_y,
        "Fleet",
        '{"Name":"Fleet' + postfix + '"}',
        mCode
      );
      break;
    case "Operation":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Level1/operation.png" class="iDesign" /><br> <span class="componentTitle">Operation` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Operation",
        0,
        1,
        pos_x,
        pos_y,
        "Operation",
        '{"Name":"Operation' + postfix + '"}',
        mCode
      );
      break;
    case "RealState":
      var mCode =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Level1/RealState.png" class="iDesign" /><br> <span class="componentTitle">RealState` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "RealState",
        0,
        1,
        pos_x,
        pos_y,
        "RealState",
        '{"Name":"Real Estate' + postfix + '"}',
        mCode
      );
      break;

    //
    case "AmmoniaPlant":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/AmmoniaPlant.png" class="iDesign" /><br> <span class="componentTitle">Ammonia Plant` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "AmmoniaPlant",
        1,
        1,
        pos_x,
        pos_y,
        "AmmoniaPlant",
        '{"Name":"Ammonia Plant' + postfix + '"}',
        battery
      );
      break;
    case "AssetReplacement":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/AssetReplacement.png" class="iDesign" /><br> <span class="componentTitle">Asset Replacement` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "AssetReplacement",
        1,
        1,
        pos_x,
        pos_y,
        "AssetReplacement",
        '{"Name":"Asset Replacement' + postfix + '"}',
        battery
      );
      break;
    case "GreenH2":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/GreenH2.png" class="iDesign" /><br> <span class="componentTitle">Green H2` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "GreenH2",
        1,
        1,
        pos_x,
        pos_y,
        "GreenH2",
        '{"Name":"GreenH2' + postfix + '"}',
        battery
      );
      break;
    case "GeoThermal":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/GeoThermal.png" class="iDesign" /><br> <span class="componentTitle">GeoThermal` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "GeoThermal",
        1,
        1,
        pos_x,
        pos_y,
        "GeoThermal",
        '{"Name":"GeoThermal' + postfix + '"}',
        battery
      );
      break;
    case "Refinery":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Refinery.png" class="iDesign" /><br> <span class="componentTitle">Refinery` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Refinery",
        1,
        1,
        pos_x,
        pos_y,
        "Refinery",
        '{"Name":"Refinery' + postfix + '"}',
        battery
      );
      break;
    case "UpStream":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/UpStream.png" class="iDesign" /><br> <span class="componentTitle">UpStream` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "UpStream",
        1,
        1,
        pos_x,
        pos_y,
        "UpStream",
        '{"Name":"UpStream' + postfix + '"}',
        battery
      );
      break;
    case "FootPrint":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/footprint.png" class="iDesign" /><br> <span class="componentTitle">Carbon Footprint` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Footprint",
        0,
        1,
        pos_x,
        pos_y,
        "Footprint",
        '{"Name":"Carbon Footprint' + postfix + '"}',
        battery
      );
      break;
    case "Retrofit":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/retrofit.png" class="iDesign" /><br> <span class="componentTitle">Retrofit` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Retrofit",
        0,
        1,
        pos_x,
        pos_y,
        "Retrofit",
        '{"Name":"Retrofit' + postfix + '"}',
        battery
      );
      break;
    case "CrProject":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/CrProject.png" class="iDesign" /><br> <span class="componentTitle">CR Project` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "CrProject",
        0,
        1,
        pos_x,
        pos_y,
        "CrProject",
        '{"Name":"CR Project' + postfix + '"}',
        battery
      );
      break;

    case "Balancer":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/icons/loadBalancer.png" class="iDesign" /><br> <span class="componentTitle">Balancer` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Balancer",
        5,
        1,
        pos_x,
        pos_y,
        "Balancer",
        '{"Name":"Balancer' + postfix + '"}',
        battery
      );
      break;
    case "EmissionData":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Emission.png" class="iDesign" /><br> <span class="componentTitle">Production Well Emission Data` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "ProductionWellEmission",
        0,
        1,
        pos_x,
        pos_y,
        "ProductionWellEmission",
        '{"Name":"ProductionVendorEmission' + postfix + '"}',
        battery
      );
      break;
    case "EmissionSaving":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/saving.png" class="iDesign" /><br> <span class="componentTitle">Emission Saving` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "EmissionSaving",
        1,
        0,
        pos_x,
        pos_y,
        "EmissionSaving",
        '{"Name":"EmissionSaving' + postfix + '"}',
        battery
      );
      break;
    case "TotalOut":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/TotalOutput.png" class="iDesign" /><br> <span class="componentTitle">Total Emission Out` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "TotalOut",
        1,
        0,
        pos_x,
        pos_y,
        "TotalOut",
        '{"Name":"TotalOut' + postfix + '"}',
        battery
      );
      break;
    case "Budget":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Budget.png" class="iDesign" /><br> <span class="componentTitle">Budget` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Budget",
        1,
        1,
        pos_x,
        pos_y,
        "Budget",
        '{"Name":"Budget' + postfix + '"}',
        battery
      );
      break;
    case "Capex":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Level1/Capex.png" class="iDesign" /><br> <span class="componentTitle">Capex/Opex` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "Capex",
        1,
        1,
        pos_x,
        pos_y,
        "Capex",
        '{"Name":"Capex/Opex' + postfix + '"}',
        battery
      );
      break;
    case "EmissionReductionTarget":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Level1/EmissionReductionTarget.png" class="iDesign" /><br> <span class="componentTitle">Emission Reduction Target` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "EmissionReductionTarget",
        1,
        1,
        pos_x,
        pos_y,
        "EmissionReductionTarget",
        '{"Name":"EmissionReductionTarget' + postfix + '"}',
        battery
      );
      break;
    case "VendorEmission":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/Carbon/Vendor.png" class="iDesign" /><br> <span class="componentTitle">Vendor Emission Data` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "VendorEmission",
        0,
        1,
        pos_x,
        pos_y,
        "VendorEmission",
        '{"Name":"VendorEmissionData' + postfix + '"}',
        battery
      );
      break;

    //
    case "battery":
      var battery =
        `
            <div class="innerDiv" priority="0">
              <div class="title-box"><img draggable="false" src="../Images/icons/Battery.png" class="iDesign" /><br> <span class="componentTitle">Battery` +
        postfix +
        `</span></div>
            </div>
            `;
      editor.addNode(
        "battery",
        1,
        1,
        pos_x,
        pos_y,
        "battery",
        '{"Name":"battery' + postfix + '"}',
        battery
      );
      break;
    case "turbine":
      var slackchat =
        `
             <div class="innerDiv" priority="2">
              <div class="title-box"><img draggable="false" src="../Images/icons/Turbine.png" class="iDesign" /><br> <span class="componentTitle">Turbines` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "turbine",
        0,
        1,
        pos_x,
        pos_y,
        "turbine",
        '{"Name":"turbines' + postfix + '"}',
        slackchat
      );
      break;
    case "Solar":
      var slackchat =
        `
            <div class="innerDiv" priority="0">
               <div class="title-box"><img draggable="false" src="../Images/icons/solar.png" class="iDesign" /><br> <span class="componentTitle">Solar Panel` +
        postfix +
        `</span> </div>
            </div>
              `;
      editor.addNode(
        "Solar",
        0,
        1,
        pos_x,
        pos_y,
        "Solar",
        '{"Name":"Solar Panel' + postfix + '"}',
        slackchat
      );
      break;
    case "Grid":
      var slackchat =
        `
           <div class="innerDiv" priority="3">
              <div class="title-box"><img draggable="false" src="../Images/icons/Grid.png" class="iDesign" /><br> <span class="componentTitle">Grid` +
        postfix +
        `</span> </div>
            </div>
              `;
      editor.addNode(
        "Grid",
        1,
        1,
        pos_x,
        pos_y,
        "Grid",
        '{"Name":"Grid' + postfix + '"}',
        slackchat
      );
      break;
    case "cloud":
      var slackchat =
        `
            <div class="innerDiv" priority="5">
              <div class="title-box"><img draggable="false" src="../Images/icons/cloud.png" class="iDesign" /><br> <span class="componentTitle">Cloud` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "cloud",
        1,
        0,
        pos_x,
        pos_y,
        "cloud",
        '{"Name":"cloud' + postfix + '"}',
        slackchat
      );
      break;
    case "WaterStorage":
      var slackchat =
        `
            <div class="innerDiv" priority="5">
              <div class="title-box"><img draggable="false" src="../Images/icons/WaterStorage.png" class="iDesign" /><br> <span class="componentTitle">Water Storage` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "WaterStorage",
        1,
        1,
        pos_x,
        pos_y,
        "WaterStorage",
        '{"Name":"Water Storage' + postfix + '"}',
        slackchat
      );
      break;
    case "GreenH2Storage":
      var slackchat =
        `
            <div class="innerDiv" priority="7">
              <div class="title-box"><img draggable="false" src="../Images/icons/StorageH2.png" class="iDesign" /><br> <span class="componentTitle">Green H2 Storage` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "GreenH2Storage",
        1,
        1,
        pos_x,
        pos_y,
        "GreenH2Storage",
        '{"Name":"Green H2 Storage' + postfix + '"}',
        slackchat
      );
      break;
    case "ExternalGraySupplier":
      var slackchat =
        `
            <div class="innerDiv" priority="7">
              <div class="title-box"><img draggable="false" src="../Images/icons/GrayStorageH2.png" class="iDesign" /><br> <span class="componentTitle">External Gray Supplier` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "ExternalGraySupplier",
        0,
        1,
        pos_x,
        pos_y,
        "ExternalGraySupplier",
        '{"Name":"External Gray Supplier' + postfix + '"}',
        slackchat
      );
      break;
    case "Mobility":
      var slackchat =
        `
            <div class="innerDiv" priority="8">
              <div class="title-box"><img draggable="false" src="../Images/icons/Mobility.png" class="iDesign" /><br> <span class="componentTitle">Mobility` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "Mobility",
        1,
        0,
        pos_x,
        pos_y,
        "Mobility",
        '{"Name":"Mobility' + postfix + '"}',
        slackchat
      );
      break;
    case "industriese":
      var slackchat =
        `
            <div class="innerDiv" priority="8">
              <div class="title-box"><img draggable="false" src="../Images/icons/industriese.png" class="iDesign" /><br> <span class="componentTitle">Industries` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "industriese",
        1,
        0,
        pos_x,
        pos_y,
        "industriese",
        '{"Name":"industries' + postfix + '"}',
        slackchat
      );
      break;
    case "electrolyzer":
      var slackchat =
        `
            <div class="innerDiv" priority="4">
              <div class="title-box"><img draggable="false" src="../Images/icons/electrolyzer.png" class="iDesign" /><br> <span class="componentTitle">Electrolyser` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "electrolyzer",
        2,
        2,
        pos_x,
        pos_y,
        "electrolyzer",
        '{"Name":"electrolyser' + postfix + '"}',
        slackchat
      );
      break;
    case "electricityLB":
      var slackchat =
        `
            <div class="innerDiv" priority="1">
              <div class="title-box"><img draggable="false" src="../Images/icons/loadBalancerElectricity.png" class="iDesign" /><br> <span class="componentTitle">Electricity LB` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "electricityLB",
        4,
        3,
        pos_x,
        pos_y,
        "electricityLB",
        '{"Name":"electricity LB' + postfix + '"}',
        slackchat
      );
      break;
    case "hydrogenLB":
      var slackchat =
        `
            <div class="innerDiv" priority="6">
              <div class="title-box"><img draggable="false" src="../Images/icons/loadBalancer.png" class="iDesign" /><br> <span class="componentTitle">Hydrogen LB` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "hydrogenLB",
        3,
        3,
        pos_x,
        pos_y,
        "hydrogenLB",
        '{"Name":"hydrogen LB' + postfix + '"}',
        slackchat
      );
      break;
    case "Optimization":
      var slackchat =
        `
            <div class="innerDiv" priority="6">
              <div class="title-box"><img draggable="false" src="../Images/icons/optimization.png" class="iDesign" /><br> <span class="componentTitle">Optimization` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "Optimization",
        3,
        3,
        pos_x,
        pos_y,
        "Optimization",
        '{"Name":"Optimization' + postfix + '"}',
        slackchat
      );
      break;
    case "Prob":
      var slackchat =
        `
            <div class="innerDiv" priority="6">
              <div class="title-box">` +
        `<table style="margin-left:-8px;" class="table table-bordered" cellpadding="0"><tr><td colspan="2" style="background: #4a8b78; color: white;">Time:</td></tr><tr><td style="background: blue; color: white;">Porperty</td><td style="background: blue; color: white;">Value</td></tr> </table>` +
        `<br> <span class="componentTitle">Prob` +
        postfix +
        `</span></div>
            </div>
              `;
      editor.addNode(
        "Optimization",
        1,
        0,
        pos_x,
        pos_y,
        "Optimization",
        '{"Name":"Prob' + postfix + '"}',
        slackchat
      );
      break;
    default:
      if (name.indexOf("userObject_") != -1) {
        postfix = createPostiFixForComponent(name.replace("userObject_", ""));
        var $element = jQuery(
          '.userDefinedObjectIcons[customName="' + name + '"]'
        );
        var htmlCode =
          '<div class="innerDiv" priority="100"><div class="title-box">';
        htmlCode +=
          '<div class="title-box"><img draggable="false" src="' +
          $element.find("img").attr("src") +
          '" class="iDesign" /><br> <span class="componentTitle">' +
          name.replace("userObject_", "") +
          postfix +
          "</span></div>" +
          "</div>";
        //
        var coInfo = userDefinedObjectsList.filter(function (el) {
          return el.name === name.replace("userObject_", "");
        });
        //
        editor.addNode(
          name.replace("userObject_", ""),
          coInfo[0].inputCount,
          coInfo[0].outputCount,
          pos_x,
          pos_y,
          name.replace("userObject_", ""),
          '{"Name":"' + name.replace("userObject_", "") + postfix + '"}',
          htmlCode
        );
      } else if (name.indexOf("Macro_") != -1) {
        var mID = jQuery('.userMacroIcons[customName="' + name + '"]').attr(
          "mID"
        );
        var mMacro = userMacrosList.filter(function (el) {
          return el.id === mID;
        });
        jQuery(mMacro[0].components).each(function (Index, Val) {
          var mProperties = {};
          //
          jQuery(Val.propertiese).each(function (proIndex, proVal) {
            mProperties[proVal.name] = proVal.value;
          });
          //
          editor.addNode(
            Val.nodeName,
            Val.inputConnectionsCount,
            Val.outputConnectionsCount,
            Val.positionTop,
            Val.positionLeft,
            Val.nodeName,
            JSON.stringify(mProperties),
            Val.nodeHtmlContent,
            false,
            Val.nodeID
          );
        });
        //
        //Create Connections
        jQuery(mMacro[0].components).each(function (Index, Val) {
          if (Val.inputConnections.length > 0) {
            jQuery(Val.inputConnections).each(function (conIndex, conVal) {
              editor.addConnection(
                conVal.node,
                Val.nodeID,
                conVal.source,
                conVal.destination
              );
            });
          }
          //if (Val.inputConnections.length > 0) {
          //    jQuery(Val.outputConnections).each(function (conIndex, conVal) {
          //        editor.addConnection(Val.nodeID, conVal.node, conVal.destination, conVal.source);
          //    })
          //}
        });
      }
      break;
  }
}
var transform = "";
function showpopup(e) {
  e.target.closest(".drawflow-node").style.zIndex = "9999";
  e.target.children[0].style.display = "block";
  //document.getElementById("modalfix").style.display = "block";
  //e.target.children[0].style.transform = 'translate('+translate.x+'px, '+translate.y+'px)';
  transform = editor.precanvas.style.transform;
  editor.precanvas.style.transform = "";
  editor.precanvas.style.left = editor.canvas_x + "px";
  editor.precanvas.style.top = editor.canvas_y + "px";
  //e.target.children[0].style.top  =  -editor.canvas_y - editor.container.offsetTop +'px';
  //e.target.children[0].style.left  =  -editor.canvas_x  - editor.container.offsetLeft +'px';
  editor.editor_mode = "fixed";
}

function closemodal(e) {
  e.target.closest(".drawflow-node").style.zIndex = "2";
  e.target.parentElement.parentElement.style.display = "none";
  //document.getElementById("modalfix").style.display = "none";
  editor.precanvas.style.transform = transform;
  editor.precanvas.style.left = "0px";
  editor.precanvas.style.top = "0px";
  editor.editor_mode = "edit";
}

function changeModule(event) {
  var all = document.querySelectorAll(".menu ul li");
  for (var i = 0; i < all.length; i++) {
    all[i].classList.remove("selected");
  }
  event.target.classList.add("selected");
}

function changeMode(option) {
  //console.log(lock.id);
  if (option == "lock") {
    lock.style.display = "none";
    unlock.style.display = "block";
  } else {
    lock.style.display = "block";
    unlock.style.display = "none";
  }
}

//#endregion DraFlow
