/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/bootstrap-tagsinput",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginBootstrapTagsinput=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),Tagsinput=function(_Plugin){function Tagsinput(){return babelHelpers.classCallCheck(this,Tagsinput),babelHelpers.possibleConstructorReturn(this,(Tagsinput.__proto__||Object.getPrototypeOf(Tagsinput)).apply(this,arguments))}return babelHelpers.inherits(Tagsinput,_Plugin),babelHelpers.createClass(Tagsinput,[{key:"getName",value:function(){return"tagsinput"}}],[{key:"getDefaults",value:function(){return{tagClass:"badge badge-default"}}}]),Tagsinput}(_Plugin3.default);_Plugin3.default.register("tagsinput",Tagsinput),exports.default=Tagsinput});