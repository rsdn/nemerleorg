/*
	Copyright (c) 2011 Vladimir Kochetkov, http://vkochetkov.blogspot.com
	
	Permission is hereby granted, free of charge, to any person obtaining
	a copy of this software and associated documentation files (the
	"Software"), to deal in the Software without restriction, including
	without limitation the rights to use, copy, modify, merge, publish,
	distribute, sublicense, and/or sell copies of the Software, and to
	permit persons to whom the Software is furnished to do so, subject to
	the following conditions:
	
	The above copyright notice and this permission notice shall be
	included in all copies or substantial portions of the Software.
	
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
	NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
	LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
	OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
	WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
(function($){
  $.getCss = $.getCss || function(url){
    var link = $("<link>");
    link.attr({
            type: 'text/css',
            rel:  'stylesheet',
            href:  url
    });
    $("head").append(link); 
		return $;
	};

	$.prettify = function(options) {
		var settings = $.extend({}, $.prettify.defaultOptions, options);

    if (settings.autoLoad) {
      var langsToLoad = settings.additionalLanguages.length;
      $.getScript(settings.prettifyUrl + 'prettify.js', function(){
        for(var i in settings.additionalLanguages)
          $.getScript(settings.prettifyUrl + 'lang-' + settings.additionalLanguages[i]+'.js', function(){langsToLoad--});
      });
      if (settings.theme != '') $.getCss(settings.themesUrl + 'theme-' + settings.theme + '.css');
    }

		(function(){
      if ((langsToLoad == 0) && typeof prettyPrint !== 'undefined') {
          $('pre.prettyprint').each(function(){
            var	$code = $(this);
     				if (settings.lineNums) $code.addClass('linenums');
            if (settings.alternateLines) $code.addClass('alternate');
            if (settings.wrapLines) $code.addClass('wraplines');
          })
          prettyPrint();
      } else {
      	setTimeout(arguments.callee, 100);
      }
		})();

    return $;
	};

	$.prettify.defaultOptions = {
    'autoLoad': true,
    'themesUrl' : '',
    'prettifyUrl': 'http://google-code-prettify.googlecode.com/svn/trunk/src/',
    'additionalLanguages': [],
    'theme': 'default',
    'lineNums': true,
    'alternateLines': true,
    'wrapLines': false
	};
})(jQuery);
