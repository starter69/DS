/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/jt-timepicker",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginJtTimepicker=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),Timepicker=function(_Plugin){function Timepicker(){return babelHelpers.classCallCheck(this,Timepicker),babelHelpers.possibleConstructorReturn(this,(Timepicker.__proto__||Object.getPrototypeOf(Timepicker)).apply(this,arguments))}return babelHelpers.inherits(Timepicker,_Plugin),babelHelpers.createClass(Timepicker,[{key:"getName",value:function(){return"timepicker"}}],[{key:"getDefaults",value:function(){return{}}}]),Timepicker}(_Plugin3.default);_Plugin3.default.register("timepicker",Timepicker),exports.default=Timepicker});