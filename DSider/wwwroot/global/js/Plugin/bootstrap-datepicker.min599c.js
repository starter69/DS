/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/bootstrap-datepicker",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginBootstrapDatepicker=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),Datepicker=function(_Plugin){function Datepicker(){return babelHelpers.classCallCheck(this,Datepicker),babelHelpers.possibleConstructorReturn(this,(Datepicker.__proto__||Object.getPrototypeOf(Datepicker)).apply(this,arguments))}return babelHelpers.inherits(Datepicker,_Plugin),babelHelpers.createClass(Datepicker,[{key:"getName",value:function(){return"datepicker"}}],[{key:"getDefaults",value:function(){return{autoclose:!0}}}]),Datepicker}(_Plugin3.default);_Plugin3.default.register("datepicker",Datepicker),exports.default=Datepicker});