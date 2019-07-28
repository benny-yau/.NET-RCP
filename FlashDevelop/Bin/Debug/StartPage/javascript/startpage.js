/**
* Templates used by the start page.
*/
var rssFeedItemTemplate = "<li><span class=\"rssItemTitle\"><a href=\"javascript:window.external.ShowURL('{1}');\">{0}</a></span><span class=\"rssItemContent\">{2}</span></li>";

/**
* Downloads XML document from url.
*/
function loadXmlDocument(url, callback)
{
	var loader = new ActiveXObject("Microsoft.XMLDOM");
	loader.onreadystatechange = function()
	{
		if (loader.readyState == 4) 
		{
			var status = loader.parseError.errorCode;
			callback(loader, status);
		}
	}
	loader.load(url);
}

/**
* Parses the rss feed xml document.
*/
function handleRssFeedXml(xml, status)
{
	var html = "";
	if (status == 0)
	{
		var items = new Array();
		var xmlItems = xml.getElementsByTagName("item");
		var xmlTitle = getNodeText(xml.getElementsByTagName("title"));
		document.getElementById("rssTitle").innerHTML = xmlTitle;
		for (var i = 0; i < xmlItems.length; i++)
		{
			var title = getNodeText(xmlItems[i].getElementsByTagName("title"));
			var link = getNodeText(xmlItems[i].getElementsByTagName("link")).replace(/\\/g, "\\\\");
			var desc = getNodeText(xmlItems[i].getElementsByTagName("description"));
			html += formatString(rssFeedItemTemplate, title, link, desc);
			if (i != xmlItems.length - 1) html += "<hr />";
		}
	} 
	else html = getLocaleString("rssFeedNotAvailable");
	var element = document.getElementById("rssContent");
	element.innerHTML = "<ul>" + html + "</ul>";
}

/**
* Safe text extraction
*/
function getNodeText(nodes)
{
	if (nodes == null) return ""; //"#ERR#1";
	if (nodes.length == 0) return ""; //"#ERR#2";
	if (nodes[0].firstChild == null) return ""; //"#ERR#3";
	return nodes[0].firstChild.nodeValue;
}

/**
* Gets the localized text for the id.
*/
function getLocaleString(id)
{
	var lang = getUrlParameter("l");
	return locale[lang + "." + id] || id;
}

/**
* Gets the value of the specified url parameter.
*/
function getUrlParameter(id)
{
	id = id.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
	var regex = new RegExp("[?&]" + id + "=([^&#]*)");
	var results = regex.exec(unescape(window.location.href));
	if (results == null) return "";
	else return results[1];
}

/**
* Formats the string with the specified arguments.
*/
function formatString(text)
{
	var result = text;
	for (var i = 1; i < arguments.length; i++)
	{
		var pattern = "{" + (i - 1) + "}";
		while (result.indexOf(pattern) >= 0)
		{
			result = result.replace(pattern, arguments[i]);
		}
	}
	return result;
}

/**
* Handles the data sent by FlashDevelop.
*/
function handleXmlData(rssUrl)
{
	if (rssUrl != null)
	{
		loadXmlDocument(rssUrl, handleRssFeedXml);
	}
}
