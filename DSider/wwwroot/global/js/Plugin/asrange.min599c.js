/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/asrange",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginAsrange=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),AsRange=function(_Plugin){function AsRange(){return babelHelpers.classCallCheck(this,AsRange),babelHelpers.possibleConstructorReturn(this,(AsRange.__proto__||Object.getPrototypeOf(AsRange)).apply(this,arguments))}return babelHelpers.inherits(AsRange,_Plugin),babelHelpers.createClass(AsRange,[{key:"getName",value:function(){return"asRange"}}],[{key:"getDefaults",value:function(){return{tip:!1,scale:!1}}}]),AsRange}(_Plugin3.default);_Plugin3.default.register("asRange",AsRange),exports.default=AsRange});