/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/jquery-labelauty",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginJqueryLabelauty=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),Labelauty=function(_Plugin){function Labelauty(){return babelHelpers.classCallCheck(this,Labelauty),babelHelpers.possibleConstructorReturn(this,(Labelauty.__proto__||Object.getPrototypeOf(Labelauty)).apply(this,arguments))}return babelHelpers.inherits(Labelauty,_Plugin),babelHelpers.createClass(Labelauty,[{key:"getName",value:function(){return"labelauty"}}],[{key:"getDefaults",value:function(){return{same_width:!0}}}]),Labelauty}(_Plugin3.default);_Plugin3.default.register("labelauty",Labelauty),exports.default=Labelauty});