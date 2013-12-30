jQuery.fn.appendEach = function( arrayOfWrappers ){
	var rawArray = jQuery.map(
	arrayOfWrappers,
	function( value, index ){
		return( value.get() ); 
	});
	this.append( rawArray );
	return( this );
};

function externalLinks() {  
	if (!document.getElementsByTagName) return;  
	var anchors = document.getElementsByTagName("a");  
	for (var i=0; i<anchors.length; i++) {  
		var anchor = anchors[i];  
		if (anchor.getAttribute("href") && anchor.getAttribute("rel") == "external")
			anchor.target = "_blank";
	}
}

window.onload = externalLinks;

function spamBlock(a,b,c) {
    var locationstring;
    if (c == "mail") {
		locationstring="mailto:"+a+"@"+b;
	} else {
		locationstring=a+b;
	}
	window.location=locationstring;
}