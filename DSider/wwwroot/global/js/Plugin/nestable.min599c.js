/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/Plugin/nestable",["exports","Plugin"],factory);else if("undefined"!=typeof exports)factory(exports,require("Plugin"));else{var mod={exports:{}};factory(mod.exports,global.Plugin),global.PluginNestable=mod.exports}}(this,function(exports,_Plugin2){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _Plugin3=babelHelpers.interopRequireDefault(_Plugin2),Nestable=function(_Plugin){function Nestable(){return babelHelpers.classCallCheck(this,Nestable),babelHelpers.possibleConstructorReturn(this,(Nestable.__proto__||Object.getPrototypeOf(Nestable)).apply(this,arguments))}return babelHelpers.inherits(Nestable,_Plugin),babelHelpers.createClass(Nestable,[{key:"getName",value:function(){return"nestable"}}],[{key:"getDefaults",value:function(){return{}}}]),Nestable}(_Plugin3.default);_Plugin3.default.register("nestable",Nestable),exports.default=Nestable});