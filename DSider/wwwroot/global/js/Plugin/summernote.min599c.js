/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/summernote",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginSummernote=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),Summernote=function(_Plugin){function Summernote(){return babelHelpers.classCallCheck(this,Summernote),babelHelpers.possibleConstructorReturn(this,(Summernote.__proto__||Object.getPrototypeOf(Summernote)).apply(this,arguments))}return babelHelpers.inherits(Summernote,_Plugin),babelHelpers.createClass(Summernote,[{key:"getName",value:function(){return"summernote"}}],[{key:"getDefaults",value:function(){return{height:300}}}]),Summernote}(_Plugin3.default);_Plugin3.default.register("summernote",Summernote),exports.default=Summernote});