/*!
 * Remark (http://getbootstrapadmin.com/remark)
 * Copyright 2017 amazingsurge
 * Licensed under the Themeforest Standard Licenses
 */

!function(global,factory){if("function"==typeof define&&define.amd)define("/App/Forum",["exports","BaseApp"],factory);else if("undefined"!=typeof exports)factory(exports,require("BaseApp"));else{var mod={exports:{}};factory(mod.exports,global.BaseApp),global.AppForum=mod.exports}}(this,function(exports,_BaseApp2){"use strict";function getInstance(){return instance||(instance=new AppForum),instance}Object.defineProperty(exports,"__esModule",{value:!0}),exports.getInstance=exports.run=exports.AppForum=void 0;var AppForum=function(_BaseApp){function AppForum(){return babelHelpers.classCallCheck(this,AppForum),babelHelpers.possibleConstructorReturn(this,(AppForum.__proto__||Object.getPrototypeOf(AppForum)).apply(this,arguments))}return babelHelpers.inherits(AppForum,_BaseApp),AppForum}(babelHelpers.interopRequireDefault(_BaseApp2).default),instance=null;exports.default=AppForum,exports.AppForum=AppForum,exports.run=function(){getInstance().run()},exports.getInstance=getInstance});