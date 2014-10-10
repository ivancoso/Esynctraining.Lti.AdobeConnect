@inherits Umbraco.Web.Mvc.UmbracoTemplatePage

{
    var user = (Umbraco.Web.PublishedCache.MemberPublishedContent)Members.GetCurrentMember();
    var email = user.Email;
    var firstName = user.FirstName;
    var lastName = user.LastName;
}

    <!-- Begin modal -->
<div class="modal fade referral-modal-lg" tabindex="-1" role="dialog">
  <div class="modal-wrapper">
      <div class="modal-dialog modal-dialog-full">
          <div class="modal-content">
            <div class="modal-header">
              <button type="button" class="close" data-dismiss="modal">
                <span aria-hidden="true">&times;</span><span class="sr-only">Close</span>
              </button>
            </div>
            <div class="modal-body">
        <!-- Begin FS-form -->
              <div class="fs-form-wrap" id="fs-form-wrap">
                <div class="fs-title">
                  <h1>Refer a Friend</h1>
                </div>
                <ol class="fs-fields">
                <li>
                    <form id="referralform" class="fs-form fs-form-full" autocomplete="off" >
                    <input type="hidden" name="fromEmail" value="" />
                    <input type="hidden" name="fromFirstName" value="" />
                    <input type="hidden" name="fromLastName" value="" />
                    <p>Earn future savings for yourself when your friends enroll.</p>
                  
                    <div class="row">
                    <div class="col-md-6">
                    <div class="form-group">
                      <label class="fs-field-label fs-anim-upper" for="firstName" required>First Name:</label>
                      <input class="fs-anim-lower" name="firstName" type="text" placeholder="First Name"/>
                    </div>
                    </div>
                    <div class="col-md-6">
                    <div class="form-group">
                      <label class="fs-field-label fs-anim-upper" for="lastName" required>Last Name:</label>
                      <input class="fs-anim-lower" name="lastName" type="text" placeholder="Last Name"/>
                    </div>
                    </div>
                    </div>

                    <div class="row">
                    <div class="col-md-6">
                    <div class="form-group">
                      <label class="fs-field-label fs-anim-upper" for="email" required>Email Address:</label>
                      <input class="fs-anim-lower" name="email" type="text" placeholder="email@example" style="margin-top: 20px;"/>
                    </div>
                    </div>
                    <div class="col-md-6">
                    <div class="form-group">
                      <label class="fs-field-label fs-anim-upper" for="role" required>Is this individual a prospective...</label>
                      <div class="fs-radio-group fs-radio-custom clearfix fs-anim-lower">
                        <span class="pull-left"><input id="role1" name="role" type="radio" value="parent"/><label for="role1" class="radio">parent?</label></span>
                        <span class="pull-left"><input id="role2" name="role" type="radio" value="student"/><label for="role2" class="radio">student?</label></span>
                      </div><!-- /fs-radio-group -->
                    </div>
                    </div>
                    </div>

                    <div class="row">
                    <div class="col-md-6">
                      <label class="fs-field-label fs-anim-upper" for="howDoYouKnow">How do you know this individual?</label>
                      <textarea class="fs-anim-lower one-line" name="howDoYouKnow" placeholder="Describe here"></textarea>
                    </div>
                    <div class="col-md-6">
                      <label class="fs-field-label fs-anim-upper" for="message">Personal Message</label>
                      <textarea class="fs-anim-lower" name="message" placeholder="Add a message for your friend"></textarea>
                    </div>
                    </div>
                    <div class="row">
                    <div class="col-md-12">
                        <div id="messages" class="messages" style="display: none;">
                        </div>
                    </div>
                    </div>
                    
                    </form><!-- /fs-form -->
                </li>
                </ol><!-- /fs-fields -->
              </div><!-- /fs-form-wrap -->
          <!-- / FS-form -->

            </div><!-- / modal-body-->
            <div class="modal-footer">
                <div class="fs-form-full" id="fs-form-wrap">
                    <button class="btn btn-primary btn-lg btn-color2" style="float:left;" id="add-friend-button">Add a Friend</button>
                    <button class="btn btn-primary btn-lg btn-color1 fs-continue-refer" type="button" style="display:none;" id="continue-button" >Continue</button>
                    <button class="btn btn-primary btn-lg btn-color1 fs-submit-refer" type="button" id="submit-button" style="margin-left: -120px;">Send Referrals</button>
                </div>
            </div>
      </div><!-- / modal-content-->
    </div><!-- / modal-dialog-->
  </div><!-- / modal-wrapper-->
</div><!-- / modal-->
                  
                  
        <!-- js for full screen form -->
        <script src="/scripts/jquery.min.js"></script>
        <script src="/scripts/classie.js"></script>
        <script src="/scripts/fullscreenForm.js"></script>
        <script src="/scripts/bootstrapValidator.js"></script>
        <script>
            (function() {
                var validateData = [];
                var formWrap = document.getElementById( 'fs-form-wrap' );
                var form = new FForm(formWrap, {
                    submitSelector: 'submit-button',
                    continueSelector: 'continue-button'
                });
                
                document.getElementById('submit-button').addEventListener('click', function (e) {
                    var currentForm = $(formWrap).find('.fs-current form');
                    var currentNum = currentForm[0].id.substr(12) - 0;
                    
                    currentForm.find('input').each(function (i, input) {
                        validateData[currentNum].revalidateField(input.name);
                        
                    });
                    
                    
                    var canGo = currentForm.find('.form-group.has-error').length ? false : true;
                    
                    if (!canGo) {
                        e.stopImmediatePropagation();
                    }
                    
                    currentForm.find('.messages')[canGo ? 'hide' : 'show']();
                    
                    return canGo;                   
                });
                
                
                $('#submit-button').off('click').on('click', function () {
                    for (var i = 0; i < validateData.length; i++) {
                        var f = $("#referralform" + (i > 0 ? i : ""));
                        $.ajax({
                            url: 'http://64.27.12.59:8010/eCommerce/referral', 
                            type: 'post', 
                            data: f.serialize(),
                            success: function (result) {
                                if (result == 'success')
                                {
                                    f.find(".messages").text('');
                                    $('.referral-modal-lg').modal('hide');
                                    f[0].reset();
                                    f.find(".has-success").removeClass("has-success");
                                }
                                else {
                                    f.find(".messages").text(result);
                                    f.find(".messages").show();
                                }
                            }, 
                            error: function (error) {
                                f[0].reset();
                                f.find(".has-success").removeClass("has-success");
                            }
                        });
                    }

                    //$('ol.fs-fields li + li').remove();
                    //$('ol.fs-fields li').addClass('fs-current').find("input[type='text']").val("");
                    //$('ol.fs-fields li').find('input[type="radio"]').each (function(i, item) { item.checked = false; });
                    //$('ol.fs-fields li').find(".has-success").removeClass("has-success");
                    //$('ol.fs-fields li').find(".has-error").removeClass("has-error");
                    //form._reinit();
                    //$('.referral-modal-lg').modal('hide');
                    return false;
                });
                
                $('#add-friend-button').off('click').on('click', function () {
                    var currentForm = $(formWrap).find('.fs-current form');
                    var currentNum = currentForm[0].id.substr(12) - 0;
                    
                    currentForm.find('input').each(function (i, input) {
                        validateData[currentNum].revalidateField(input.name);
                        
                    });
                                        
                    var canGo = currentForm.find('.form-group.has-error').length ? false : true;
                    
                    currentForm.find('.messages')[canGo ? 'hide' : 'show']();
                    
                    if (!canGo) {
                        return;
                    }
                    
                    var formNumber = $('ol.fs-fields li')/*.removeClass('fs-current')*/.length;
                    var newForm = $('ol.fs-fields li:first').clone().removeClass('fs-current');
                    newForm.find("input[type='text']").val("");
                    newForm.find(".has-success").removeClass("has-success");
                    newForm.find(".has-error").removeClass("has-error");
                    newForm.find('#referralform').attr('id','referralform' + formNumber);
                    newForm.find('#messages').attr('id','messages' + formNumber).empty();
                    newForm.find('#role1').each(function(i, item) { item.id = 'role1' + formNumber; item.checked = false; });
                    newForm.find('#role2').each(function(i, item) { item.id = 'role2' + formNumber; item.checked = false; });
                    newForm.find('[for="role1"]').attr('for', 'role1' + formNumber);
                    newForm.find('[for="role2"]').attr('for', 'role2' + formNumber);
                    
                    $('ol.fs-fields').append(newForm);
                    
                    var validate = addValidator(newForm.find('form'), '#messages' + formNumber);
                    validateData[formNumber] = validate;                    
                    
                    form._reinit();
                    
                    form._nextField(formNumber);

                });
                

                var addValidator = function(f, container) {
                    f.bootstrapValidator({
                    trigger: 'blur focus keyup',
                    container: container,
                    fields: {
                        firstName: {
                            validators: {
                                notEmpty: {
                                    message: "Please enter your friend's first name"
                                }
                            }
                        },
                        lastName: {
                            validators: {
                                notEmpty: {
                                    message: "Please enter your friend's last name"
                                }
                            }
                        },
                        email: {
                            validators: {
                                notEmpty: {
                                    message: "Please enter your friend's email"
                                },
                                emailAddress: {
                                    message: 'Please enter valid email address'
                                }
                            }
                        },
                        role: {
                            validators: {
                                notEmpty: {
                                    message: 'Please select a value'
                                }
                            }
                        }
                    }});
                    
                    return f.data('bootstrapValidator');
                
                }
                var referData = addValidator($('#referralform'), '#messages');
                validateData[0] = referData;

            })();
        </script><!-- / js for full screen form -->