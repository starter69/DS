/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/clockpicker",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginClockpicker=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),Clockpicker=function(_Plugin){function Clockpicker(){return babelHelpers.classCallCheck(this,Clockpicker),babelHelpers.possibleConstructorReturn(this,(Clockpicker.__proto__||Object.getPrototypeOf(Clockpicker)).apply(this,arguments))}return babelHelpers.inherits(Clockpicker,_Plugin),babelHelpers.createClass(Clockpicker,[{key:"getName",value:function(){return"clockpicker"}}],[{key:"getDefaults",value:function(){return{donetext:"Done"}}}]),Clockpicker}(_Plugin3.default);_Plugin3.default.register("clockpicker",Clockpicker),exports.default=Clockpicker});