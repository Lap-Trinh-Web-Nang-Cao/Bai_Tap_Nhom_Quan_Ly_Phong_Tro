/**
 * Auth JavaScript - Xử lý tương tác cho trang Đăng Ký/Đăng Nhập
 */

(function () {
    'use strict';

    // ===== TOGGLE PASSWORD VISIBILITY =====
    document.addEventListener('DOMContentLoaded', function () {
        const toggleButtons = document.querySelectorAll('.toggle-password');

        toggleButtons.forEach(button => {
            button.addEventListener('click', function (e) {
                e.preventDefault();
                const target = this.getAttribute('data-target');
                const input = document.getElementById(target);
                const icon = this.querySelector('i');

                if (input && icon) {
                    if (input.type === 'password') {
                        input.type = 'text';
                        icon.classList.remove('fa-eye');
                        icon.classList.add('fa-eye-slash');
                    } else {
                        input.type = 'password';
                        icon.classList.remove('fa-eye-slash');
                        icon.classList.add('fa-eye');
                    }
                }
            });
        });
    });

    // ===== FILE UPLOAD PREVIEW =====
    document.addEventListener('DOMContentLoaded', function () {
        const fileInput = document.getElementById('giayToFiles');
        
        if (fileInput) {
            fileInput.addEventListener('change', function (e) {
                const preview = document.getElementById('filePreview');
                const files = e.target.files;
                
                if (preview) {
                    preview.innerHTML = '';
                    
                    if (files.length > 0) {
                        Array.from(files).forEach(file => {
                            if (file.type.startsWith('image/')) {
                                const reader = new FileReader();
                                
                                reader.onload = function (event) {
                                    const img = document.createElement('img');
                                    img.src = event.target.result;
                                    img.alt = file.name;
                                    preview.appendChild(img);
                                };
                                
                                reader.readAsDataURL(file);
                            }
                        });
                        
                        // Update label text
                        const label = document.querySelector('.file-label span');
                        if (label) {
                            label.textContent = `${files.length} file(s) đã chọn`;
                        }
                    }
                }
            });
        }
    });

    // ===== FORM VALIDATION =====
    function validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    }

    function validatePhone(phone) {
        const re = /^(\+84|0)[0-9]{9,10}$/;
        return re.test(phone.replace(/\s/g, ''));
    }

    // ===== AUTO FOCUS NEXT FIELD ON ENTER =====
    document.addEventListener('DOMContentLoaded', function () {
        const inputs = document.querySelectorAll('.form-control');
        
        inputs.forEach(input => {
            input.addEventListener('keypress', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    const formGroups = Array.from(document.querySelectorAll('.form-group'));
                    const currentGroup = this.closest('.form-group');
                    const currentIndex = formGroups.indexOf(currentGroup);
                    
                    if (currentIndex < formGroups.length - 1) {
                        const nextInput = formGroups[currentIndex + 1].querySelector('.form-control');
                        if (nextInput) {
                            nextInput.focus();
                        }
                    } else {
                        // If last field, submit the form
                        const form = this.closest('form');
                        if (form) {
                            form.submit();
                        }
                    }
                }
            });
        });
    });

    // ===== RADIO BUTTON STYLING =====
    document.addEventListener('DOMContentLoaded', function () {
        const identityOptions = document.querySelectorAll('.identity-option');
        
        identityOptions.forEach(option => {
            option.addEventListener('click', function () {
                const radio = this.querySelector('input[type="radio"]');
                if (radio) {
                    radio.checked = true;
                    
                    // Update styling for all options
                    identityOptions.forEach(opt => {
                        opt.style.borderColor = '#E0E0E0';
                        opt.style.background = 'white';
                    });
                    
                    // Highlight selected option
                    this.style.borderColor = '#4A90E2';
                    this.style.background = '#E3F2FD';
                }
            });
        });
    });

    // ===== SMOOTH SCROLL =====
    document.addEventListener('DOMContentLoaded', function () {
        const authFormWrapper = document.querySelector('.auth-form-wrapper');
        
        if (authFormWrapper) {
            authFormWrapper.style.scrollBehavior = 'smooth';
        }
    });

    // ===== SOCIAL LOGIN HANDLERS (Placeholder) =====
    document.addEventListener('DOMContentLoaded', function () {
        const googleBtn = document.querySelector('.btn-google');
        const facebookBtn = document.querySelector('.btn-facebook');
        
        if (googleBtn) {
            googleBtn.addEventListener('click', function (e) {
                e.preventDefault();
                console.log('Google login clicked');
                // TODO: Implement Google OAuth
                alert('Tính năng đăng nhập bằng Google đang được phát triển');
            });
        }
        
        if (facebookBtn) {
            facebookBtn.addEventListener('click', function (e) {
                e.preventDefault();
                console.log('Facebook login clicked');
                // TODO: Implement Facebook OAuth
                alert('Tính năng đăng nhập bằng Facebook đang được phát triển');
            });
        }
    });

    // ===== AUTO DISMISS ALERTS =====
    document.addEventListener('DOMContentLoaded', function () {
        const alerts = document.querySelectorAll('.alert-success, .alert-info');
        
        alerts.forEach(alert => {
            setTimeout(() => {
                alert.style.transition = 'opacity 0.5s ease';
                alert.style.opacity = '0';
                setTimeout(() => {
                    alert.remove();
                }, 500);
            }, 5000); // Dismiss after 5 seconds
        });
    });

    // ===== FORM SUBMIT LOADING STATE =====
    document.addEventListener('DOMContentLoaded', function () {
        const authForms = document.querySelectorAll('.auth-form');
        
        authForms.forEach(form => {
            form.addEventListener('submit', function () {
                const submitBtn = this.querySelector('.btn-login, .btn-register');
                if (submitBtn) {
                    submitBtn.disabled = true;
                    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i><span>Đang xử lý...</span>';
                }
            });
        });
    });

    // ===== INPUT REAL-TIME VALIDATION =====
    document.addEventListener('DOMContentLoaded', function () {
        const emailInput = document.getElementById('Email');
        
        if (emailInput) {
            emailInput.addEventListener('blur', function () {
                const value = this.value.trim();
                if (value && !validateEmail(value) && !validatePhone(value)) {
                    this.style.borderColor = '#EF5350';
                    
                    // Add error message if not exists
                    let errorMsg = this.parentElement.querySelector('.validation-hint');
                    if (!errorMsg) {
                        errorMsg = document.createElement('small');
                        errorMsg.className = 'validation-hint';
                        errorMsg.style.color = '#EF5350';
                        errorMsg.style.fontSize = '12px';
                        errorMsg.textContent = 'Email hoặc số điện thoại không hợp lệ';
                        this.parentElement.appendChild(errorMsg);
                    }
                } else {
                    this.style.borderColor = '#E0E0E0';
                    const errorMsg = this.parentElement.querySelector('.validation-hint');
                    if (errorMsg) {
                        errorMsg.remove();
                    }
                }
            });
        }
    });

    // ===== SELLER REGISTRATION MULTI-STEP =====
    document.addEventListener('DOMContentLoaded', function () {
        const sellerForm = document.getElementById('sellerRegisterForm');
        
        if (sellerForm) {
            let currentStep = 1;
            const totalSteps = 4;
            
            const btnPrev = document.getElementById('btnPrev');
            const btnNext = document.getElementById('btnNext');
            const btnSubmit = document.getElementById('btnSubmit');
            
            function updateStep(step) {
                // Hide all steps
                document.querySelectorAll('.step-content').forEach(content => {
                    content.classList.remove('active');
                });
                
                // Show current step
                document.querySelector(`.step-content[data-step="${step}"]`).classList.add('active');
                
                // Update progress
                document.querySelectorAll('.progress-step').forEach((progressStep, index) => {
                    progressStep.classList.remove('active', 'completed');
                    if (index + 1 < step) {
                        progressStep.classList.add('completed');
                    } else if (index + 1 === step) {
                        progressStep.classList.add('active');
                    }
                });
                
                // Update progress line
                const progressLine = document.getElementById('progressLine');
                const progress = ((step - 1) / (totalSteps - 1)) * 100;
                progressLine.style.width = progress + '%';
                
                // Update buttons
                btnPrev.style.display = step > 1 ? 'inline-flex' : 'none';
                btnNext.style.display = step < totalSteps ? 'inline-flex' : 'none';
                btnSubmit.style.display = step === totalSteps ? 'inline-flex' : 'none';
                
                currentStep = step;
            }
            
            btnNext.addEventListener('click', function () {
                if (currentStep < totalSteps) {
                    updateStep(currentStep + 1);
                }
            });
            
            btnPrev.addEventListener('click', function () {
                if (currentStep > 1) {
                    updateStep(currentStep - 1);
                }
            });
            
            // Initialize
            updateStep(currentStep);
        }
    });

    // Expose functions globally
    window.validateEmail = validateEmail;
    window.validatePhone = validatePhone;

})();