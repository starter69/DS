/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/bootstrap-maxlength",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginBootstrapMaxlength=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),Maxlength=function(_Plugin){function Maxlength(){return babelHelpers.classCallCheck(this,Maxlength),babelHelpers.possibleConstructorReturn(this,(Maxlength.__proto__||Object.getPrototypeOf(Maxlength)).apply(this,arguments))}return babelHelpers.inherits(Maxlength,_Plugin),babelHelpers.createClass(Maxlength,[{key:"getName",value:function(){return"maxlength"}}],[{key:"getDefaults",value:function(){return{warningClass:"badge badge-warning",limitReachedClass:"badge badge-danger"}}}]),Maxlength}(_Plugin3.default);_Plugin3.default.register("maxlength",Maxlength),exports.default=Maxlength});