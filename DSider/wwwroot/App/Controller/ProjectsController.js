var Tariana = angular.module('Tariana', []);
var selectedProjectdID = '';
var selectedSubProjectID = '';
Tariana.controller("ProjectsController", function ($scope) {
    //Create suitable url to show dashboards(Carbon Mitigation OR Green Hydrogen)
    $scope.createDynamicUrl = function (id, name, projectID, subProject_Type) {
        if (subProject_Type != 'Carbon_Mitigation')
            window.location.href = '/Home/Dashboard/' + id + '/' + name + '/' + projectID + '/' + subProject_Type;
        else
            window.location.href = '/Home/DashboardMitigation/' + id + '/' + name + '/' + projectID + '/' + subProject_Type;
    }
    //Diplay subprojects of selected project
    $scope.subProjectsSelect = function () {
        mProjectID = getParameterByName('project');
        if (mProjectID == null)
            return;
        selectedProjectdID = mProjectID;
        $.ajax({
            url: '/api/WebAPI_Projects/selectSubProject/' + selectedProjectdID,
            type: "GET",
            contentType: 'application/json',
            success: function (response) {
                $scope.subProjectsList = response;
                $scope.$apply()
            },
            error: function (response) {

            },
            failure: function (response) {

            }
        });
    };
    $scope.showSubProject = function (projectID) {
        window.history.replaceState(null, null, "?project=" + projectID);
        $scope.subProjectsSelect();
        jQuery('#SubProjectTab').prop('checked', true);
    }
    if (getParameterByName('project') != null) {
        $scope.showSubProject(getParameterByName('project'));
    }
    //When come with back button from simulation
    $scope.subProjectsSelect();
    $scope.projectsList = [];
    $scope.subProjectsList = [];
    //Show all projects
    $scope.projectsSelect = function () {
        $.ajax({
            url: '/api/WebAPI_Projects/selectProject',
            type: "GET",
            contentType: 'application/json',
            success: function (response) {
                $scope.projectsList = response;
                $scope.$apply();
            },
            error: function (response) {

            },
            failure: function (response) {

            }
        });
    };
    $scope.projectsSelect();
    //Backward from sub projects list to projects list
    $scope.backToProject = function () {
        jQuery('#projectTab').prop('checked', true);
    };
    //Show add new project modal
    $scope.addNewProjects = function () {
        selectedProjectdID = '';
        jQuery('#addNewProject').modal('show');
    };
    //Send project data to web api
    $scope.saveProject = function () {
        $.ajax({
            url: '/api/WebAPI_Projects/saveProject/' + jQuery('#txtProjectName').val() + '/' + jQuery('#txtDescription').val() + '/' + (selectedProjectdID == '' ? '0' : selectedProjectdID),
            type: "GET",
            contentType: 'application/json',
            success: function (response) {
                $scope.projectsSelect();
                selectedProjectdID = '';
                jQuery('#addNewProject').modal('hide');
                alertify.success('Project saved');
                jQuery('#txtProjectName,#txtDescription').val('');
            },
            error: function (response) {

            },
            failure: function (response) {

            }
        })
    };
    //When new project modal is getting hide
    jQuery('#addNewProject').on('hidden.bs.modal', function () {
        jQuery('#txtProjectName,#txtDescription').val('');
    });
    //Show add new sub project(Model) modal
    $scope.addNewSubProjects = function () {
        selectedSubProjectID = '';
        jQuery('#addNewSubProject').modal('show');
    };
    //Send sub project(Model) data to web api
    $scope.saveSubProject = function () {
        if (jQuery('#txtSubProjectName').val() == '') {
            alert('Name is required.');
            jQuery('#txtSubProjectName').focus();
            return;
        }
        if (jQuery('#txtSubProjectDescription').val() == '') {
            alert('Description is required.');
            jQuery('#txtSubProjectDescription').focus();
            return;
        }
        $.ajax({
            url: '/api/WebAPI_Projects/saveSubProject/' + jQuery('#txtSubProjectName').val() + '/' +
                jQuery('#txtSubProjectDescription').val() + '/' + selectedProjectdID.toString() + '/' + (selectedSubProjectID == '' ? '0' : selectedSubProjectID) +
                '/' + jQuery('#ddSubProject_Type').val(),
            type: "GET",
            contentType: 'application/json',
            success: function (response) {
                $scope.subProjectsSelect();
                alertify.success('Item saved');
                jQuery('#txtSubProjectName,#txtSubProjectDescription').val('');
                selectedSubProjectID = '';
                jQuery('#addNewSubProject').modal('hide');
            },
            error: function (response) {

            },
            failure: function (response) {

            }
        })
    };
    //Go to simulation page(Model Design Page)
    $scope.goToSimulation = function (projectID, name, folder, subProject_Type) {
        window.location.href = '/Home/Simulation?project=' + projectID + '&name=' + name + '&folder=' + folder + '&type=' + subProject_Type;
    };
    //Show sub project data in text boxes with specific sub project id
    $scope.editSubProject = function (id) {
        var currentSubProject = $scope.subProjectsList.filter(function (el) {
            return el.id === id;
        });
        if (currentSubProject.length > 0) {
            selectedSubProjectID = currentSubProject[0].id;
            jQuery('#txtSubProjectName').val(currentSubProject[0].name);
            jQuery('#txtSubProjectDescription').val(currentSubProject[0].description);
            jQuery('#ddSubProject_Type').val(currentSubProject[0].subProject_Type);
            jQuery('#addNewSubProject').modal('show');
        }
    };
    //When new sub project modal is getting hide
    jQuery('#addNewSubProject').on('hidden.bs.modal', function () {
        jQuery('#txtSubProjectName,#txtSubProjectDescription').val('');
        selectedSubProjectID = '';
    });
    //Delete selected sub project by id
    $scope.deleteSubProject = function (id) {
        alertify.confirm("Are you sure to delete?", function (e) {
            if (e) {
                $.ajax({
                    url: '/api/WebAPI_Projects/deleteSubProject/' + id,
                    type: "GET",
                    contentType: 'application/json',
                    success: function (response) {
                        alertify.log('Item deleted.');
                        $scope.subProjectsSelect();
                    },
                    error: function (response) {
                    },
                    failure: function (response) {
                    }
                });
            }
        });
    };
    //Show project data to text boxes with specific id
    $scope.editProject = function (id) {
        var currentProject = $scope.projectsList.filter(function (el) {
            return el.projectID === id;
        });
        if (currentProject.length > 0) {
            selectedProjectdID = id;
            jQuery('#txtProjectName').val(currentProject[0].name);
            jQuery('#txtDescription').val(currentProject[0].descrption);
            jQuery('#addNewProject').modal('show');
        }
    };
    //Delete selected project with specific id
    $scope.deleteProject = function (id) {
        alertify.confirm("Are you sure to delete?", function (e) {
            if (e) {
                $.ajax({
                    url: '/api/WebAPI_Projects/deleteProject/' + id,
                    type: "GET",
                    contentType: 'application/json',
                    success: function (response) {
                        alertify.log('Project deleted.');
                        $scope.projectsSelect();
                    },
                    error: function (response) {
                    },
                    failure: function (response) {
                    }
                });
            }
        });
    };
    $scope.sharedProjectsHistory = [];
    //Share sub project
    $scope.shareSubProject = function (id, name) {
        selectedSubProjectID = id;
        jQuery('#shareSubPorjectTitle').text('Share: ' + name);
        $.ajax({
            url: '/api/WebAPI_Projects/getSharedHistoryBySubProjectID/' + selectedSubProjectID,
            type: "GET",
            contentType: 'application/json',
            success: function (response) {
                console.log(response)
                $scope.sharedProjectsHistory = response;
                $scope.$apply();
            },
            error: function (response) {

            },
            failure: function (response) {

            }
        });
        jQuery('#chbDuplicateShare').prop('checked', false);
        jQuery('#shareSubProject').modal('show');
    };
    //Delete share history with specific id
    $scope.deleteShare = function (id) {
        alertify.confirm("Are you sure to delete?", function (e) {
            if (e) {
                $.ajax({
                    url: '/api/WebAPI_Projects/deleteSharedProject/' + id,
                    type: "GET",
                    contentType: 'application/json',
                    success: function (response) {
                        alertify.log('Item deleted.');
                        $scope.shareSubProject(selectedSubProjectID, jQuery('#shareSubPorjectTitle').text().replaceAll('Share: ', ''));
                        $scope.$apply();
                    },
                    error: function (response) {
                    },
                    failure: function (response) {
                    }
                });
            }
        });
    }
    //Send sub projecct info an user name to share
    $scope.saveShareSubProject = function () {
        var permissionStatus = jQuery('input[name=sharePermissionRadio]:checked').val();
        //
        $.ajax({
            url: '/api/WebAPI_Projects/shareSubProject/' + selectedSubProjectID + '/' + jQuery('#txtShareUserName').val() + '/' + permissionStatus + '/' +
                jQuery('#chbDuplicateShare').is(':checked'),
            type: "GET",
            contentType: 'application/json',
            success: function (response) {
                jQuery('#shareSubProject').modal('hide');
                alertify.log(response);
                jQuery('#txtShareUserName').val('');
            },
            error: function (response) {

            },
            failure: function (response) {

            }
        });
    };
    //Show upload json file modal
    $scope.uploadJSONFile = function (id, Name) {
        jQuery('#uploadJSONModal').modal('show');
        selectedSubProjectID = id;
    };
    //Duplicate sub project
    $scope.copySubProject = function (id, Name) {
        alertify.confirm("Are you sure to duplicate?", function (e) {
            if (e) {
                $.ajax({
                    url: '/api/WebAPI_Projects/duplicateSubProject/' + id,
                    type: "GET",
                    contentType: 'application/json',
                    success: function (response) {
                        alertify.log('Sub Project duplicated.');
                        $scope.subProjectsSelect();
                    },
                    error: function (response) {
                    },
                    failure: function (response) {
                    }
                });
            }
        });
    };
    var selectedItemForTransfer;
    //Show transfer sub project modal
    $scope.prepareFoTransferSubProject = function (selectedItem) {
        //jQuery('#ddProjectsForTransfer').empty();
        selectedItemForTransfer = selectedItem;
        jQuery('#subProjectNameForTransfer').text(selectedItem.name);
        jQuery('#transferSubProjectModal').modal('show');
        jQuery($scope.projectsList).each(function (Index, Val) {
            jQuery('#ddProjectsForTransfer').append('<option value="' + Val.projectID + '">' + Val.name + '</option>')
        });
        jQuery("#ddProjectsForTransfer").select2({
            placeholder: "Select Project",
            allowClear: true,
            dropdownParent: $('#transferSubProjectModal')
        });
    };
    //Send transfer sub project data to web api
    $scope.saveTransferProject = function () {
        if (selectedItemForTransfer.projectID == jQuery('#ddProjectsForTransfer').val()) {
            alertify.error('Transfer is not valid. New project and current project are same.');
            return;
        }
        $.ajax({
            url: '/api/WebAPI_Projects/TransferSubProject/' + selectedItemForTransfer.id + '/' + jQuery('#ddProjectsForTransfer').val(),
            type: "GET",
            contentType: 'application/json',
            success: function (response) {
                alertify.success('Sub projects has been trasfered.');
                $scope.subProjectsSelect();
                jQuery('#transferSubProjectModal').modal('hide');
            },
            error: function (response) {
            },
            failure: function (response) {

            }
        });
    }
    //Check status from html 
    $scope.checkApprovedStatus = function (status) {
        if (status == 'Approved')
            return true;
        else
            return false;
    }
    $scope.isShowSharedProjects = function () {
        if (getParameterByName('project') == '-1')
            return false;
        else
            return true;
    };
});
//Upload json file
function UploadFile() {
    var fileUpload = $("#files").get(0);
    var files = fileUpload.files;
    if (files.length == 0) {
        alertify.error('Select your files');
        return;
    }
    var data = new FormData();
    data.append(files[0].name, files[0]);
    createCookie('subProject', selectedSubProjectID);
    $.ajax({
        type: "POST",
        url: "/api/WebAPI_Uploader/UploadJSONFile",
        contentType: false,
        processData: false,
        data: data,
        async: false,
        beforeSend: function () {
            $("#divloader").show()
        },
        success: function (message) {
            alertify.success('JSON file imported.');
            jQuery('#uploadJSONModal').modal('hide');
        },
        error: function (x, e) {
            alert("Error!");
        },
        complete: function () {
            $("#divloader").hide();
            jQuery('#uploadJSONModal').modal('hide');
        }
    });
}