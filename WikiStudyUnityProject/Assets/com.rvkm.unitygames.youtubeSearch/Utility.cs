﻿using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Net;
using System.Text;
using System.IO;
using UnityEditor;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class Utility
    {
        /// <summary>
        /// https://stackoverflow.com/questions/31760560/collapsible-list-with-dynamically-loaded-content
        /// </summary>
        public const string upperHtml = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        ul.hide {
 display: none;
}
</style>
</head>
<body>
    <script src = ""https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"" ></script>
<div id=""container"">
  <ul id = ""nav"">
  </ul>
</div>
    <script type=""text/javascript"">
        $(document).ready(function() {
  $(""#nav"").html(loadnavigation(menu, null)); //to load the li items//
  $(""#container"").click(function(e){
            //e refers to the click event. 
            //bind it
            var eventTarget = $(e.target);

            //we need to use closest here. If you append other HTML-elements to the li, eventTarget
            //will refer to these elements. This way, it always travels up to the 
            //closest ancestor with an data-id attribute.
            eventTarget.closest(""li[data-id]"").children(""ul"").toggleClass(""hide""); //find the closest ancestor (or self) that has data-id and hide its ul.

        });
    });

//below is my own code just to show an example of my comment:

//I'm using a array, combined with objects to build the basic structure of the menu.
var menu =
[
";
        public const string lowerHtml = @"
];

function loadnavigation(obj, dataID) {
  var html = """"; //using var here is important. We want this variable to be private and within the scope of only this function.
  //this will be a recursive function.
  for (var i = 0; i < obj.length; i++) {
    var id = i;
    if (dataID != null || dataID != undefined) //we start the function at first with value null.
    {
      id = dataID + "" - "" + id; //separate id's from eachother with a dash   
    }
    var menuItem = '<li data-id=""' + id + '"">';
    menuItem += obj[i].name; //append the menu name with +=
    if (obj[i].menu) //obj has submenu
    {
      menuItem += ""<ul class='hide'>"" + loadnavigation(obj[i].menu, id) + ""</ul>"";
    }
menuItem += ""</li>""; //close
html += menuItem;
  }
  return html;
}
    </script>
</body>
</html> ";

    }
}