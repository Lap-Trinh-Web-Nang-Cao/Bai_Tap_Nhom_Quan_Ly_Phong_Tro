"use strict";

// Setting Color

$(window).resize(function() {
	$(window).width(); 
});

$('.changeBodyBackgroundFullColor').on('click', function(){
	if($(this).attr('data-color') == 'default'){
		$('body').removeAttr('data-background-full');
	} else {
		$('body').attr('data-background-full', $(this).attr('data-color'));
	}

	$(this).parent().find('.changeBodyBackgroundFullColor').removeClass("selected");
	$(this).addClass("selected");
	layoutsColors();
});

$('.changeLogoHeaderColor').on('click', function(){
	if($(this).attr('data-color') == 'default'){
		$('.logo-header').removeAttr('data-background-color');
	} else {
		$('.logo-header').attr('data-background-color', $(this).attr('data-color'));
	}

	$(this).parent().find('.changeLogoHeaderColor').removeClass("selected");
	$(this).addClass("selected");
	customCheckColor();
	layoutsColors();
});

$('.changeTopBarColor').on('click', function(){
	if($(this).attr('data-color') == 'default'){
		$('.main-header .navbar-header').removeAttr('data-background-color');
	} else {
		$('.main-header .navbar-header').attr('data-background-color', $(this).attr('data-color'));
	}

	$(this).parent().find('.changeTopBarColor').removeClass("selected");
	$(this).addClass("selected");
	layoutsColors();
});

$('.changeSideBarColor').on('click', function(){
	if($(this).attr('data-color') == 'default'){
		$('.sidebar').removeAttr('data-background-color');
	} else {
		$('.sidebar').attr('data-background-color', $(this).attr('data-color'));
	}

	$(this).parent().find('.changeSideBarColor').removeClass("selected");
	$(this).addClass("selected");
	layoutsColors();
});

$('.changeBackgroundColor').on('click', function(){
	$('body').removeAttr('data-background-color');
	$('body').attr('data-background-color', $(this).attr('data-color'));
	$(this).parent().find('.changeBackgroundColor').removeClass("selected");
	$(this).addClass("selected");
});

function customCheckColor(){
	var logoHeader = $('.logo-header').attr('data-background-color');
	var $logo = $('#mainLogo');
	
	console.log('🎨 customCheckColor() called - Background:', logoHeader);
	
	// ========================================
	// QUAN TRỌNG: CHỈ THAY ĐỔI FILTER, KHÔNG BAO GIỜ ĐỔI SRC
	// Logo LUÔN LUÔN giữ nguyên src là Content/img/logo.png
	// ========================================
	
	if (logoHeader === "white") {
		// Background trắng -> Logo màu xanh
		$logo.css('filter', 'brightness(0) saturate(100%) invert(27%) sepia(98%) saturate(2594%) hue-rotate(207deg) brightness(96%) contrast(91%)');
		console.log('✅ Logo color: BLUE (for white background)');
	} else {
		// Background màu đậm -> Logo màu trắng
		$logo.css('filter', 'brightness(0) invert(1)');
		console.log('✅ Logo color: WHITE (for colored background)');
	}
}

// ========================================
// KHỞI TẠO VÀ BẢO VỆ LOGO
// ========================================
$(document).ready(function() {
	console.log('🚀 setting-demo.js initialized');
	
	// BƯỚC 1: Đảm bảo logo luôn là file PNG
	var $logo = $('#mainLogo, .navbar-brand');
	
	// Lấy src hiện tại và kiểm tra
	var currentSrc = $logo.attr('src');
	console.log('📝 Current logo src:', currentSrc);
	
	// Nếu src không phải là logo.png, force về đúng path
	if (!currentSrc || !currentSrc.includes('logo.png')) {
		var correctSrc = '/Content/img/logo.png';
		$logo.attr('src', correctSrc);
		console.log('⚠️ Logo src was incorrect, fixed to:', correctSrc);
	}
	
	// Lưu src gốc để bảo vệ
	window.originalLogoSrc = $logo.attr('src');
	console.log('💾 Original logo src saved:', window.originalLogoSrc);
	
	// BƯỚC 2: Áp dụng màu ban đầu
	setTimeout(function() {
		customCheckColor();
		console.log('✅ Initial logo color set');
	}, 100);
	
	// BƯỚC 3: Setup MutationObserver để theo dõi thay đổi background
	var $logoHeader = $('.logo-header');
	if ($logoHeader.length > 0) {
		var observer = new MutationObserver(function(mutations) {
			mutations.forEach(function(mutation) {
				if (mutation.attributeName === 'data-background-color') {
					console.log('🔄 Logo header background changed, updating logo color...');
					customCheckColor();
				}
			});
		});
		
		observer.observe($logoHeader[0], {
			attributes: true,
			attributeFilter: ['data-background-color']
		});
		
		console.log('✅ MutationObserver setup for logo-header');
	}
	
	// BƯỚC 4: BẢO VỆ CHỐNG THAY ĐỔI SRC
	// Theo dõi mọi thay đổi trên logo
	var logoObserver = new MutationObserver(function(mutations) {
		mutations.forEach(function(mutation) {
			if (mutation.attributeName === 'src') {
				var $target = $(mutation.target);
				var currentSrc = $target.attr('src');
				
				// Nếu src bị đổi sang .svg HOẶC path không đúng, revert lại
				if (currentSrc && (currentSrc.includes('.svg') || !currentSrc.includes('logo.png'))) {
					console.warn('⚠️ Logo src was changed to:', currentSrc);
					console.warn('🔧 Reverting to correct PNG logo...');
					
					if (window.originalLogoSrc) {
						$target.attr('src', window.originalLogoSrc);
						console.log('✅ Logo src restored to:', window.originalLogoSrc);
					}
					
					// Re-apply color filter
					customCheckColor();
				}
			}
		});
	});
	
	// Observe cả #mainLogo và .navbar-brand
	$('#mainLogo, .navbar-brand').each(function() {
		if (this) {
			logoObserver.observe(this, {
				attributes: true,
				attributeFilter: ['src']
			});
		}
	});
	
	console.log('✅ Logo src protection observer activated');
	
	// BƯỚC 5: CHẶN MỌI REQUEST TỚI .SVG FILES (nếu có thể)
	// Override các hàm có thể load image
	var originalImageSrc = Object.getOwnPropertyDescriptor(HTMLImageElement.prototype, 'src');
	if (originalImageSrc && originalImageSrc.set) {
		Object.defineProperty(HTMLImageElement.prototype, 'src', {
			set: function(value) {
				// Nếu đang set logo và value là .svg, chặn lại
				if (this.id === 'mainLogo' || this.classList.contains('navbar-brand')) {
					if (value && value.includes('.svg')) {
						console.warn('🚫 Blocked attempt to set logo to SVG:', value);
						value = window.originalLogoSrc || '/Content/img/logo.png';
					}
				}
				originalImageSrc.set.call(this, value);
			},
			get: originalImageSrc.get
		});
		console.log('✅ Image src setter override installed');
	}
});

// ========================================
// CUSTOM TOGGLE SIDEBAR
// ========================================
var toggle_customSidebar = false,
custom_open = 0;

if(!toggle_customSidebar) {
	var toggle = $('.custom-template .custom-toggle');

	toggle.on('click', (function(){
		if (custom_open == 1){
			$('.custom-template').removeClass('open');
			toggle.removeClass('toggled');
			custom_open = 0;
		}  else {
			$('.custom-template').addClass('open');
			toggle.addClass('toggled');
			custom_open = 1;
		}
	})
	);
	toggle_customSidebar = true;
}