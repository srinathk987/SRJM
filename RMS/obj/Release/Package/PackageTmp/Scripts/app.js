
 

var app = angular.module('RMS', ['ngSanitize', 'ui.select']);
app.filter('propsFilter', function () {

    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            var keys = Object.keys(props);

            items.forEach(function (item) {
                var itemMatches = false;

                for (var i = 0; i < keys.length; i++) {
                    var prop = keys[i];
                    var text = props[prop].toLowerCase();
                    if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                        itemMatches = true;
                        break;
                    }
                }

                if (itemMatches) {
                    out.push(item);
                }
            });
        } else {
            // Let the output be the input untouched
            out = items;
        }

        return out;
    };
});
//app.factory('myService', function () {
//    var savedData = {}
//    function set(data) {
//        savedData = data;
//    }
//    function get() {
//        return savedData;
//    }

//    return {
//        set: set,
//        get: get
//    }

//});

app.factory('myService', function () {
    return {

        Redirect: function () {
            return window.location.href = "/POSTransaction/DashBoard";
        }


    };
});

app.service('MenuService', function () {

    var Menudata = {
        ParentName: '',
        ChildName: ''
    };
    return Menudata;

})

app.directive('kitPropercase', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            ngModelCtrl.$parsers.push(function (text) {
                ;
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var sspace = text.replace(/ +/g, ' ');
                if (text != sspace)
                    text = sspace;
                // text = text.replace(/[  ]/g, '');
                var newstr = text.split(' ');
                for (i = 0; i < newstr.length; i++) {
                    var copy = newstr[i].substring(1).toLowerCase();
                    newstr[i] = newstr[i][0].toUpperCase() + copy;
                }
                newstr = newstr.join(" ");

                ngModelCtrl.$setViewValue(newstr);
                ngModelCtrl.$render();

                return newstr;

            });
        }
    };

});

app.directive('autoComplete', function ($timeout) {
    debugger;
    return function (scope, iElement, iAttrs) {
        debugger;
        iElement.autocomplete({
            source: scope[iAttrs.uiItems],
            select: function () {
                $timeout(function () {
                    iElement.trigger('input');
                }, 0);
            }
        });
    };
});
app.directive('angucompleteAlt', ['$q', '$parse', '$http', '$sce', '$timeout', '$templateCache', '$interpolate', function ($q, $parse, $http, $sce, $timeout, $templateCache, $interpolate) {
    // keyboard events
    var KEY_DW = 40;
    var KEY_RT = 39;
    var KEY_UP = 38;
    var KEY_LF = 37;
    var KEY_ES = 27;
    var KEY_EN = 13;
    var KEY_TAB = 9;

    var MIN_LENGTH = 3;
    var MAX_LENGTH = 524288;  // the default max length per the html maxlength attribute
    var PAUSE = 500;
    var BLUR_TIMEOUT = 200;

    // string constants
    var REQUIRED_CLASS = 'autocomplete-required';
    var TEXT_SEARCHING = 'Searching...';
    var TEXT_NORESULTS = 'No results found';
    var TEMPLATE_URL = '/angucomplete-alt/index.html';

    // Set the default template for this directive
    $templateCache.put(TEMPLATE_URL,
        '<div class="angucomplete-holder" ng-class="{\'angucomplete-dropdown-visible\': showDropdown}">' +
        '  <input id="{{id}}_value" name="{{inputName}}" tabindex="{{fieldTabindex}}" ng-class="{\'angucomplete-input-not-empty\': notEmpty}" ng-model="searchStr" ng-disabled="disableInput" type="{{inputType}}" placeholder="{{placeholder}}" maxlength="{{maxlength}}" ng-focus="onFocusHandler()" class="{{inputClass}}" ng-focus="resetHideResults()" ng-blur="hideResults($event)" autocapitalize="off" autocorrect="off" autocomplete="off" ng-change="inputChangeHandler(searchStr)"/>' +
        '  <div id="{{id}}_dropdown" class="angucomplete-dropdown" ng-show="showDropdown">' +
        '    <div class="angucomplete-searching" ng-show="searching" ng-bind="textSearching"></div>' +
        '    <div class="angucomplete-searching" ng-show="!searching && (!results || results.length == 0)" ng-bind="textNoResults"></div>' +
        '    <div class="angucomplete-row" ng-repeat="result in results" ng-click="selectResult(result)" ng-mouseenter="hoverRow($index)" ng-class="{\'angucomplete-selected-row\': $index == currentIndex}">' +
        '      <div ng-if="imageField" class="angucomplete-image-holder">' +
        '        <img ng-if="result.image && result.image != \'\'" ng-src="{{result.image}}" class="angucomplete-image"/>' +
        '        <div ng-if="!result.image && result.image != \'\'" class="angucomplete-image-default"></div>' +
        '      </div>' +
        '      <div class="angucomplete-title" ng-if="matchClass" ng-bind-html="result.title"></div>' +
        '      <div class="angucomplete-title" ng-if="!matchClass">{{ result.title }}</div>' +
        '      <div ng-if="matchClass && result.description && result.description != \'\'" class="angucomplete-description" ng-bind-html="result.description"></div>' +
        '      <div ng-if="!matchClass && result.description && result.description != \'\'" class="angucomplete-description">{{result.description}}</div>' +
        '    </div>' +
        '  </div>' +
        '</div>'
    );

    function link(scope, elem, attrs, ctrl) {
        var inputField = elem.find('input');
        var minlength = MIN_LENGTH;
        var searchTimer = null;
        var hideTimer;
        var requiredClassName = REQUIRED_CLASS;
        var responseFormatter;
        var validState = null;
        var httpCanceller = null;
        var dd = elem[0].querySelector('.angucomplete-dropdown');
        var isScrollOn = false;
        var mousedownOn = null;
        var unbindInitialValue;
        var displaySearching;
        var displayNoResults;

        elem.on('mousedown', function (event) {
            if (event.target.id) {
                mousedownOn = event.target.id;
                if (mousedownOn === scope.id + '_dropdown') {
                    document.body.addEventListener('click', clickoutHandlerForDropdown);
                }
            }
            else {
                mousedownOn = event.target.className;
            }
        });

        scope.currentIndex = scope.focusFirst ? 0 : null;
        scope.searching = false;
        unbindInitialValue = scope.$watch('initialValue', function (newval) {
            if (newval) {
                // remove scope listener
                unbindInitialValue();
                // change input
                handleInputChange(newval, true);
            }
        });

        scope.$watch('fieldRequired', function (newval, oldval) {
            if (newval !== oldval) {
                if (!newval) {
                    ctrl[scope.inputName].$setValidity(requiredClassName, true);
                }
                else if (!validState || scope.currentIndex === -1) {
                    handleRequired(false);
                }
                else {
                    handleRequired(true);
                }
            }
        });

        scope.$on('angucomplete-alt:clearInput', function (event, elementId) {
            if (!elementId || elementId === scope.id) {
                scope.searchStr = null;
                callOrAssign();
                handleRequired(false);
                clearResults();
            }
        });

        scope.$on('angucomplete-alt:changeInput', function (event, elementId, newval) {
            if (!!elementId && elementId === scope.id) {
                handleInputChange(newval);
            }
        });

        function handleInputChange(newval, initial) {
            if (newval) {
                if (typeof newval === 'object') {
                    scope.searchStr = extractTitle(newval);
                    callOrAssign({ originalObject: newval });
                } else if (typeof newval === 'string' && newval.length > 0) {
                    scope.searchStr = newval;
                } else {
                    if (console && console.error) {
                        console.error('Tried to set ' + (!!initial ? 'initial' : '') + ' value of angucomplete to', newval, 'which is an invalid value');
                    }
                }

                handleRequired(true);
            }
        }

        // #194 dropdown list not consistent in collapsing (bug).
        function clickoutHandlerForDropdown(event) {
            mousedownOn = null;
            scope.hideResults(event);
            document.body.removeEventListener('click', clickoutHandlerForDropdown);
        }

        // for IE8 quirkiness about event.which
        function ie8EventNormalizer(event) {
            return event.which ? event.which : event.keyCode;
        }

        function callOrAssign(value) {
            if (typeof scope.selectedObject === 'function') {
                scope.selectedObject(value, scope.selectedObjectData);
            }
            else {
                scope.selectedObject = value;
            }

            if (value) {
                handleRequired(true);
            }
            else {
                handleRequired(false);
            }
        }

        function callFunctionOrIdentity(fn) {
            return function (data) {
                return scope[fn] ? scope[fn](data) : data;
            };
        }

        function setInputString(str) {
            callOrAssign({ originalObject: str });

            if (scope.clearSelected) {
                scope.searchStr = null;
            }
            clearResults();
        }

        function extractTitle(data) {
            // split title fields and run extractValue for each and join with ' '
            return scope.titleField.split(',')
              .map(function (field) {
                  return extractValue(data, field);
              })
              .join(' ');
        }

        function extractValue(obj, key) {
            var keys, result;
            if (key) {
                keys = key.split('.');
                result = obj;
                for (var i = 0; i < keys.length; i++) {
                    result = result[keys[i]];
                }
            }
            else {
                result = obj;
            }
            return result;
        }

        function findMatchString(target, str) {
            var result, matches, re;
            // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_Expressions
            // Escape user input to be treated as a literal string within a regular expression
            re = new RegExp(str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'i');
            if (!target) { return; }
            if (!target.match || !target.replace) { target = target.toString(); }
            matches = target.match(re);
            if (matches) {
                result = target.replace(re,
                    '<span class="' + scope.matchClass + '">' + matches[0] + '</span>');
            }
            else {
                result = target;
            }
            return $sce.trustAsHtml(result);
        }

        function handleRequired(valid) {
            scope.notEmpty = valid;
            validState = scope.searchStr;
            if (scope.fieldRequired && ctrl && scope.inputName) {
                ctrl[scope.inputName].$setValidity(requiredClassName, valid);
            }
        }

        function keyupHandler(event) {
            var which = ie8EventNormalizer(event);
            if (which === KEY_LF || which === KEY_RT) {
                // do nothing
                return;
            }

            if (which === KEY_UP || which === KEY_EN) {
                event.preventDefault();
            }
            else if (which === KEY_DW) {
                event.preventDefault();
                if (!scope.showDropdown && scope.searchStr && scope.searchStr.length >= minlength) {
                    initResults();
                    scope.searching = true;
                    searchTimerComplete(scope.searchStr);
                }
            }
            else if (which === KEY_ES) {
                clearResults();
                scope.$apply(function () {
                    inputField.val(scope.searchStr);
                });
            }
            else {
                if (minlength === 0 && !scope.searchStr) {
                    return;
                }

                if (!scope.searchStr || scope.searchStr === '') {
                    scope.showDropdown = false;
                } else if (scope.searchStr.length >= minlength) {
                    initResults();

                    if (searchTimer) {
                        $timeout.cancel(searchTimer);
                    }

                    scope.searching = true;

                    searchTimer = $timeout(function () {
                        searchTimerComplete(scope.searchStr);
                    }, scope.pause);
                }

                if (validState && validState !== scope.searchStr && !scope.clearSelected) {
                    scope.$apply(function () {
                        callOrAssign();
                    });
                }
            }
        }

        function handleOverrideSuggestions(event) {
            if (scope.overrideSuggestions &&
                !(scope.selectedObject && scope.selectedObject.originalObject === scope.searchStr)) {
                if (event) {
                    event.preventDefault();
                }

                // cancel search timer
                $timeout.cancel(searchTimer);
                // cancel http request
                cancelHttpRequest();

                setInputString(scope.searchStr);
            }
        }

        function dropdownRowOffsetHeight(row) {
            var css = getComputedStyle(row);
            return row.offsetHeight +
              parseInt(css.marginTop, 10) + parseInt(css.marginBottom, 10);
        }

        function dropdownHeight() {
            return dd.getBoundingClientRect().top +
              parseInt(getComputedStyle(dd).maxHeight, 10);
        }

        function dropdownRow() {
            return elem[0].querySelectorAll('.angucomplete-row')[scope.currentIndex];
        }

        function dropdownRowTop() {
            return dropdownRow().getBoundingClientRect().top -
              (dd.getBoundingClientRect().top +
               parseInt(getComputedStyle(dd).paddingTop, 10));
        }

        function dropdownScrollTopTo(offset) {
            dd.scrollTop = dd.scrollTop + offset;
        }

        function updateInputField() {
            var current = scope.results[scope.currentIndex];
            if (scope.matchClass) {
                inputField.val(extractTitle(current.originalObject));
            }
            else {
                inputField.val(current.title);
            }
        }

        function keydownHandler(event) {
            var which = ie8EventNormalizer(event);
            var row = null;
            var rowTop = null;

            if (which === KEY_EN && scope.results) {
                if (scope.currentIndex >= 0 && scope.currentIndex < scope.results.length) {
                    event.preventDefault();
                    scope.selectResult(scope.results[scope.currentIndex]);
                } else {
                    handleOverrideSuggestions(event);
                    clearResults();
                }
                scope.$apply();
            } else if (which === KEY_DW && scope.results) {
                event.preventDefault();
                if ((scope.currentIndex + 1) < scope.results.length && scope.showDropdown) {
                    scope.$apply(function () {
                        scope.currentIndex++;
                        updateInputField();
                    });

                    if (isScrollOn) {
                        row = dropdownRow();
                        if (dropdownHeight() < row.getBoundingClientRect().bottom) {
                            dropdownScrollTopTo(dropdownRowOffsetHeight(row));
                        }
                    }
                }
            } else if (which === KEY_UP && scope.results) {
                event.preventDefault();
                if (scope.currentIndex >= 1) {
                    scope.$apply(function () {
                        scope.currentIndex--;
                        updateInputField();
                    });

                    if (isScrollOn) {
                        rowTop = dropdownRowTop();
                        if (rowTop < 0) {
                            dropdownScrollTopTo(rowTop - 1);
                        }
                    }
                }
                else if (scope.currentIndex === 0) {
                    scope.$apply(function () {
                        scope.currentIndex = -1;
                        inputField.val(scope.searchStr);
                    });
                }
            } else if (which === KEY_TAB) {
                if (scope.results && scope.results.length > 0 && scope.showDropdown) {
                    if (scope.currentIndex === -1 && scope.overrideSuggestions) {
                        // intentionally not sending event so that it does not
                        // prevent default tab behavior
                        handleOverrideSuggestions();
                    }
                    else {
                        if (scope.currentIndex === -1) {
                            scope.currentIndex = 0;
                        }
                        scope.selectResult(scope.results[scope.currentIndex]);
                        scope.$digest();
                    }
                }
                else {
                    // no results
                    // intentionally not sending event so that it does not
                    // prevent default tab behavior
                    if (scope.searchStr && scope.searchStr.length > 0) {
                        handleOverrideSuggestions();
                    }
                }
            } else if (which === KEY_ES) {
                // This is very specific to IE10/11 #272
                // without this, IE clears the input text
                event.preventDefault();
            }
        }

        function httpSuccessCallbackGen(str) {
            return function (responseData, status, headers, config) {
                // normalize return obejct from promise
                if (!status && !headers && !config && responseData.data) {
                    responseData = responseData.data;
                }
                scope.searching = false;
                processResults(
                  extractValue(responseFormatter(responseData), scope.remoteUrlDataField),
                  str);
            };
        }

        function httpErrorCallback(errorRes, status, headers, config) {
            scope.searching = false;

            // cancelled/aborted
            if (status === 0 || status === -1) { return; }

            // normalize return obejct from promise
            if (!status && !headers && !config) {
                status = errorRes.status;
            }
            if (scope.remoteUrlErrorCallback) {
                scope.remoteUrlErrorCallback(errorRes, status, headers, config);
            }
            else {
                if (console && console.error) {
                    console.error('http error');
                }
            }
        }

        function cancelHttpRequest() {
            if (httpCanceller) {
                httpCanceller.resolve();
            }
        }

        function getRemoteResults(str) {
            var params = {},
                url = scope.remoteUrl + encodeURIComponent(str);
            if (scope.remoteUrlRequestFormatter) {
                params = { params: scope.remoteUrlRequestFormatter(str) };
                url = scope.remoteUrl;
            }
            if (!!scope.remoteUrlRequestWithCredentials) {
                params.withCredentials = true;
            }
            cancelHttpRequest();
            httpCanceller = $q.defer();
            params.timeout = httpCanceller.promise;
            $http.get(url, params)
              .success(httpSuccessCallbackGen(str))
              .error(httpErrorCallback);
        }

        function getRemoteResultsWithCustomHandler(str) {
            cancelHttpRequest();

            httpCanceller = $q.defer();

            scope.remoteApiHandler(str, httpCanceller.promise)
              .then(httpSuccessCallbackGen(str))
              .catch(httpErrorCallback);

            /* IE8 compatible
            scope.remoteApiHandler(str, httpCanceller.promise)
              ['then'](httpSuccessCallbackGen(str))
              ['catch'](httpErrorCallback);
            */
        }

        function clearResults() {
            scope.showDropdown = false;
            scope.results = [];
            if (dd) {
                dd.scrollTop = 0;
            }
        }

        function initResults() {
            scope.showDropdown = displaySearching;
            scope.currentIndex = scope.focusFirst ? 0 : -1;
            scope.results = [];
        }

        function getLocalResults(str) {
            var i, match, s, value,
                searchFields = scope.searchFields.split(','),
                matches = [];
            if (typeof scope.parseInput() !== 'undefined') {
                str = scope.parseInput()(str);
            }
            for (i = 0; i < scope.localData.length; i++) {
                match = false;

                for (s = 0; s < searchFields.length; s++) {
                    value = extractValue(scope.localData[i], searchFields[s]) || '';
                    match = match || (value.toString().toLowerCase().indexOf(str.toString().toLowerCase()) >= 0);
                }

                if (match) {
                    matches[matches.length] = scope.localData[i];
                }
            }
            return matches;
        }

        function checkExactMatch(result, obj, str) {
            if (!str) { return false; }
            for (var key in obj) {
                if (obj[key].toLowerCase() === str.toLowerCase()) {
                    scope.selectResult(result);
                    return true;
                }
            }
            return false;
        }

        function searchTimerComplete(str) {
            // Begin the search
            if (!str || str.length < minlength) {
                return;
            }
            if (scope.localData) {
                scope.$apply(function () {
                    var matches;
                    if (typeof scope.localSearch() !== 'undefined') {
                        matches = scope.localSearch()(str, scope.localData);
                    } else {
                        matches = getLocalResults(str);
                    }
                    scope.searching = false;
                    processResults(matches, str);
                });
            }
            else if (scope.remoteApiHandler) {
                getRemoteResultsWithCustomHandler(str);
            } else {
                getRemoteResults(str);
            }
        }

        function processResults(responseData, str) {
            var i, description, image, text, formattedText, formattedDesc;

            if (responseData && responseData.length > 0) {
                scope.results = [];

                for (i = 0; i < responseData.length; i++) {
                    if (scope.titleField && scope.titleField !== '') {
                        text = formattedText = extractTitle(responseData[i]);
                    }

                    description = '';
                    if (scope.descriptionField) {
                        description = formattedDesc = extractValue(responseData[i], scope.descriptionField);
                    }

                    image = '';
                    if (scope.imageField) {
                        image = extractValue(responseData[i], scope.imageField);
                    }

                    if (scope.matchClass) {
                        formattedText = findMatchString(text, str);
                        formattedDesc = findMatchString(description, str);
                    }

                    scope.results[scope.results.length] = {
                        title: formattedText,
                        description: formattedDesc,
                        image: image,
                        originalObject: responseData[i]
                    };
                }

            } else {
                scope.results = [];
            }

            if (scope.autoMatch && scope.results.length === 1 &&
                checkExactMatch(scope.results[0],
                  { title: text, desc: description || '' }, scope.searchStr)) {
                scope.showDropdown = false;
            } else if (scope.results.length === 0 && !displayNoResults) {
                scope.showDropdown = false;
            } else {
                scope.showDropdown = true;
            }
        }

        function showAll() {
            if (scope.localData) {
                processResults(scope.localData, '');
            }
            else if (scope.remoteApiHandler) {
                getRemoteResultsWithCustomHandler('');
            }
            else {
                getRemoteResults('');
            }
        }

        scope.onFocusHandler = function () {
            if (scope.focusIn) {
                scope.focusIn();
            }
            if (minlength === 0 && (!scope.searchStr || scope.searchStr.length === 0)) {
                scope.currentIndex = scope.focusFirst ? 0 : scope.currentIndex;
                scope.showDropdown = true;
                showAll();
            }
        };

        scope.hideResults = function () {
            if (mousedownOn &&
                (mousedownOn === scope.id + '_dropdown' ||
                 mousedownOn.indexOf('angucomplete') >= 0)) {
                mousedownOn = null;
            }
            else {
                hideTimer = $timeout(function () {
                    clearResults();
                    scope.$apply(function () {
                        if (scope.searchStr && scope.searchStr.length > 0) {
                            inputField.val(scope.searchStr);
                        }
                    });
                }, BLUR_TIMEOUT);
                cancelHttpRequest();

                if (scope.focusOut) {
                    scope.focusOut();
                }

                if (scope.overrideSuggestions) {
                    if (scope.searchStr && scope.searchStr.length > 0 && scope.currentIndex === -1) {
                        handleOverrideSuggestions();
                    }
                }
            }
        };

        scope.resetHideResults = function () {
            if (hideTimer) {
                $timeout.cancel(hideTimer);
            }
        };

        scope.hoverRow = function (index) {
            scope.currentIndex = index;
        };

        scope.selectResult = function (result) {
            // Restore original values
            if (scope.matchClass) {
                result.title = extractTitle(result.originalObject);
                result.description = extractValue(result.originalObject, scope.descriptionField);
            }

            if (scope.clearSelected) {
                scope.searchStr = null;
            }
            else {
                scope.searchStr = result.title;
            }
            callOrAssign(result);
            clearResults();
        };

        scope.inputChangeHandler = function (str) {
            if (str.length < minlength) {
                cancelHttpRequest();
                clearResults();
            }
            else if (str.length === 0 && minlength === 0) {
                scope.searching = false;
                showAll();
            }

            if (scope.inputChanged) {
                str = scope.inputChanged(str);
            }
            return str;
        };

        // check required
        if (scope.fieldRequiredClass && scope.fieldRequiredClass !== '') {
            requiredClassName = scope.fieldRequiredClass;
        }

        // check min length
        if (scope.minlength && scope.minlength !== '') {
            minlength = parseInt(scope.minlength, 10);
        }

        // check pause time
        if (!scope.pause) {
            scope.pause = PAUSE;
        }

        // check clearSelected
        if (!scope.clearSelected) {
            scope.clearSelected = false;
        }

        // check override suggestions
        if (!scope.overrideSuggestions) {
            scope.overrideSuggestions = false;
        }

        // check required field
        if (scope.fieldRequired && ctrl) {
            // check initial value, if given, set validitity to true
            if (scope.initialValue) {
                handleRequired(true);
            }
            else {
                handleRequired(false);
            }
        }

        scope.inputType = attrs.type ? attrs.type : 'text';

        // set strings for "Searching..." and "No results"
        scope.textSearching = attrs.textSearching ? attrs.textSearching : TEXT_SEARCHING;
        scope.textNoResults = attrs.textNoResults ? attrs.textNoResults : TEXT_NORESULTS;
        displaySearching = scope.textSearching === 'false' ? false : true;
        displayNoResults = scope.textNoResults === 'false' ? false : true;

        // set max length (default to maxlength deault from html
        scope.maxlength = attrs.maxlength ? attrs.maxlength : MAX_LENGTH;

        // register events
        inputField.on('keydown', keydownHandler);
        inputField.on('keyup', keyupHandler);

        // set response formatter
        responseFormatter = callFunctionOrIdentity('remoteUrlResponseFormatter');

        // set isScrollOn
        $timeout(function () {
            var css = getComputedStyle(dd);
            isScrollOn = css.maxHeight && css.overflowY === 'auto';
        });
    }

    return {
        restrict: 'EA',
        require: '^?form',
        scope: {
            selectedObject: '=',
            selectedObjectData: '=',
            disableInput: '=',
            initialValue: '=',
            localData: '=',
            localSearch: '&',
            remoteUrlRequestFormatter: '=',
            remoteUrlRequestWithCredentials: '@',
            remoteUrlResponseFormatter: '=',
            remoteUrlErrorCallback: '=',
            remoteApiHandler: '=',
            id: '@',
            type: '@',
            placeholder: '@',
            textSearching: '@',
            textNoResults: '@',
            remoteUrl: '@',
            remoteUrlDataField: '@',
            titleField: '@',
            descriptionField: '@',
            imageField: '@',
            inputClass: '@',
            pause: '@',
            searchFields: '@',
            minlength: '@',
            matchClass: '@',
            clearSelected: '@',
            overrideSuggestions: '@',
            fieldRequired: '=',
            fieldRequiredClass: '@',
            inputChanged: '=',
            autoMatch: '@',
            focusOut: '&',
            focusIn: '&',
            fieldTabindex: '@',
            inputName: '@',
            focusFirst: '@',
            parseInput: '&'
        },
        templateUrl: function (element, attrs) {
            return attrs.templateUrl || TEMPLATE_URL;
        },
        compile: function (tElement) {
            var startSym = $interpolate.startSymbol();
            var endSym = $interpolate.endSymbol();
            if (!(startSym === '{{' && endSym === '}}')) {
                var interpolatedHtml = tElement.html()
                  .replace(/\{\{/g, startSym)
                  .replace(/\}\}/g, endSym);
                tElement.html(interpolatedHtml);
            }
            return link;
        }
    };
}]);



app.controller('rmsCtrl', function ($http, $scope, $rootScope, MenuService) {
    debugger
    $scope.MenuDetails = MenuService;
    //$scope.ParentName = 'POS';
    //$scope.menuname = 'Home';

    $http.post('/Login/CheckUserData').success(function (data) {
        if (data == "NO") {
            window.location.href = "/Login/Login";
        }
    });

    $rootScope.msgSave = "Are You Sure? Do You Want To Save?";
    $rootScope.msgUpdate = "Are You Sure? Do You Want To Update?";
    $rootScope.msgDelete = "Are You Sure? Do You Want To Delete?";
    $rootScope.msgCEdit = "Cannot Edit as it is in use by the application";
    $rootScope.msgCDelete = "Cannot Delete as it is in use by the application";
    $rootScope.Save = "Details Saved Successfully";
    $rootScope.items = [];
    $scope.LoadModuleNames = function () {
        $http({
            url: '/Login/ShowModuleNames/',
            method: "get"
        }).success(function (data) {
            debugger;



            var ModuleNames = [];
            for (var z = 0; z < data.ModuleNames.length; z++) {

                ModuleNames.push(data.ModuleNames[z].ParentModuleName);
            }
            //$scope.ParentModule = $.unique(data.ModuleNames.map(function (d) {
            //    return d.ParentModuleName;
            //}));
            $scope.ParentModule = ModuleNames.filter(function (elem, pos) {
                return ModuleNames.indexOf(elem) == pos;
            });




            //  $scope.XYZ = 'MMS';
            // $scope.ParentName = mtable;
            $scope.ModuleNames = data.ModuleNames;
            $scope.username = data.username;
            $scope.menuname = data.modulename;
            if (data.status == "NO") {
                window.location.href = "/Login/Login";
            }
        });

    }

    $http({
        url: '/POSTransaction/getParentModuleData',
        method: 'get'

    }).success(function (data) {
        //alert(data);
        $scope.ParentName = data;
    })

    $scope.getChildClass = function (functionname) {
        if (functionname == $scope.childname)
            return "active";
        else
            return "";
    }
    $scope.getMenuClass = function (modulename) {

        if (modulename == $scope.menuname)
            return "main-menu-active";
        else
            return "";

    }
    $scope.ChildClick = function (x) {

        $rootScope.linkname = x.FunctionName;
        $http.post('/Login/GetChildName', { ChildName: x.FunctionName }).success(function (data) {
            window.location.href = x.FunctionUrl;
        });
    }
    //$scope.Activemodulename = "panel panel-bg ";
    $scope.ParentModuleClick = function (mtable) {
        debugger;
        $scope.menuname = '';
        // $scope.XYZ = mtable;
        $scope.ParentName = mtable;

        $http.post('/POSTransaction/ParentModuleData', { ParentName: mtable }).success(function (data) {

        });


    }
    //$scope.ActiveClass = function (mtable) {
    //    debugger

    //    if (mtable == $scope.ParentName) {
    //        return 'active';
    //    }
    //    //else {

    //    //}

    //}

    $scope.ModuleNameClick = function (x) {

        var ModuleId = x.ModuleId;

        $http.post('/Login/BindChildNames', { ModuleId: ModuleId, modulename: x.ModuleName }).success(function (data) {

            if (data.userid > 0) {
                window.location.href = data.url;
                $scope.childname = data.childname;
                $scope.menuname = data.modulename;
            }
            else {
                window.location.href = "/Login/Login";
            }
        });
    }


    $scope.LoadChildNames = function () {
        $scope.LoadModuleNames();
        $http({
            url: '/Login/ShowChildNames/',
            method: "get"
        }).success(function (data) {
            debugger

            $scope.ChildNames = data.ChildNames;
            $scope.ModuleNames = data.ModuleNames;
            $scope.username = data.username;
            $scope.childname = data.childname;
            $scope.menuname = data.modulename;
            // $scope.Sri = data.modulename;
            if (data.status == "NO") {
                window.location.href = "/Login/Login";
            }
        });
    }

    activePanel = $("#accordion-nav div.panel:first");
    $(activePanel).addClass('active');





});

app.directive('ktFocus', function () {
    return {
        restrict: 'A',
        link: function ($scope, elem, attrs) {
            elem.bind('keydown', function (e) {

                var code = e.keyCode || e.which;
                if (code === 13) {
                    e.preventDefault();
                    var $this = $(e.target);
                    var index = parseFloat($this.attr('data-index'));
                    $('[data-index="' + (index + 1).toString() + '"]').focus();

                }
            });
        }
    }
});

//app.directive('ngMax', function () {
//    debugger;
//    return {
//        restrict: 'A',
//        require: 'ngModel',
//        link: function (scope, elem, attr, ctrl) {
//            debugger;
//            scope.$watch(attr.ngMax, function () {
//                ctrl.$setViewValue(ctrl.$viewValue);
//            });
//            var maxValidator = function (value) {
//                var max = scope.$eval(attr.ngMax) || Infinity;
//                if (!isEmpty(value) && value > max) {
//                    ctrl.$setValidity('ngMax', false);
//                    return undefined;
//                } else {
//                    ctrl.$setValidity('ngMax', true);
//                    return value;
//                }
//            };

//            ctrl.$parsers.push(maxValidator);
//            ctrl.$formatters.push(maxValidator);
//        }
//    };
//});
app.directive('ngDropdownMultiselect', ['$filter', '$document', '$compile', '$parse',
    function ($filter, $document, $compile, $parse) {

        return {
            restrict: 'AE',
            scope: {
                kotlist: '=',
                tableid: '=',
                selectedModel: '=',
                options: '=',
                extraSettings: '=',
                events: '=',
                searchFilter: '=?',
                translationTexts: '=',
                groupBy: '@'
            },
            template: function (element, attrs) {
                var checkboxes = attrs.checkboxes ? true : false;
                var groups = attrs.groupBy ? true : false;

                var KOTS = [];

                var template = '<div class="multiselect-parent btn-group dropdown-multiselect">';
                template += '<button type="button" class="dropdown-toggle" ng-class="settings.buttonClasses" ng-click="toggleDropdown()">{{getButtonText()}}&nbsp;<span class="caret"></span></button>';
                template += '<ul class="dropdown-menu dropdown-menu-form" ng-style="{display: open ? \'block\' : \'none\', height : settings.scrollable ? settings.scrollableHeight : \'auto\' }" style="overflow: scroll" >';
                template += '<li ng-hide="!settings.showCheckAll || settings.selectionLimit > 0"><a data-ng-click="selectAll()"><span class="glyphicon glyphicon-ok"></span>  {{texts.checkAll}}</a>';
                template += '<li ng-show="settings.showUncheckAll"><a data-ng-click="deselectAll();"><span class="glyphicon glyphicon-remove"></span>   {{texts.uncheckAll}}</a></li>';
                template += '<li ng-hide="(!settings.showCheckAll || settings.selectionLimit > 0) && !settings.showUncheckAll" class="divider"></li>';
                template += '<li ng-show="settings.enableSearch"><div class="dropdown-header"><input type="text" class="form-control" style="width: 100%;" ng-model="searchFilter" placeholder="{{texts.searchPlaceholder}}" /></li>';
                template += '<li ng-show="settings.enableSearch" class="divider"></li>';

                if (groups) {
                    template += '<li ng-repeat-start="option in orderedItems | filter: searchFilter" ng-show="getPropertyForObject(option, settings.groupBy) !== getPropertyForObject(orderedItems[$index - 1], settings.groupBy)" role="presentation" class="dropdown-header">{{ getGroupTitle(getPropertyForObject(option, settings.groupBy)) }}</li>';
                    template += '<li ng-repeat-end role="presentation">';
                } else {
                    template += '<li role="presentation" ng-repeat="option in options | filter: searchFilter">';
                }

                template += '<a role="menuitem" tabindex="-1" ng-click="setSelectedItem(getPropertyForObject(option,settings.idProp))">';

                if (checkboxes) {
                    template += '<div class="checkbox"><label><input ng-disabled="selectedoptiondchk" class="checkboxInput" type="checkbox" ng-click="checkboxClick($event, getPropertyForObject(option,settings.idProp))" ng-checked="isChecked(getPropertyForObject(option,settings.idProp))" /> {{getPropertyForObject(option, settings.displayProp)}}</label></div></a>';
                } else {
                    template += '<span data-ng-class="{\'glyphicon glyphicon-ok\': isChecked(getPropertyForObject(option,settings.idProp))}"></span> {{getPropertyForObject(option, settings.displayProp)}}</a>';
                }

                template += '</li>';

                template += '<li class="divider" ng-show="settings.selectionLimit > 1"></li>';
                template += '<li role="presentation" ng-show="settings.selectionLimit > 1"><a role="menuitem">{{selectedModel.length}} {{texts.selectionOf}} {{settings.selectionLimit}} {{texts.selectionCount}}</a></li>';

                template += '</ul>';
                template += '</div>';

                element.html(template);
            },
            link: function ($scope, $element, $attrs) {
                var $dropdownTrigger = $element.children()[0];

                $scope.toggleDropdown = function () {
                    $scope.open = !$scope.open;
                };

                $scope.checkboxClick = function ($event, id) {

                    // alert('Chckbox');
                    var KOTS = id.split("-")[0];
                    $scope.tableid = KOTS;
                    //   alert(KOTS);
                    //   alert($scope.selectedModel);
                    // $scope.text.push(KOTS);
                    $scope.setSelectedItem(id);
                    $event.stopImmediatePropagation();
                };

                $scope.externalEvents = {
                    onItemSelect: angular.noop,
                    onItemDeselect: angular.noop,
                    onSelectAll: angular.noop,
                    onDeselectAll: angular.noop,
                    onInitDone: angular.noop,
                    onMaxSelectionReached: angular.noop
                };

                $scope.settings = {
                    dynamicTitle: true,
                    scrollable: true,
                    scrollableHeight: '300px',
                    closeOnBlur: true,
                    displayProp: 'label',
                    idProp: 'id',
                    externalIdProp: 'id',
                    enableSearch: false,
                    selectionLimit: 0,
                    showCheckAll: true,
                    showUncheckAll: true,
                    closeOnSelect: false,
                    buttonClasses: 'btn btn-default',
                    closeOnDeselect: false,
                    groupBy: $attrs.groupBy || undefined,
                    groupByTextProvider: null,
                    smartButtonMaxItems: 0,
                    smartButtonTextConverter: angular.noop
                };

                $scope.texts = {
                    checkAll: 'Check All',
                    uncheckAll: 'Uncheck All',
                    selectionCount: 'checked',
                    selectionOf: '/',
                    searchPlaceholder: 'Search...',
                    buttonDefaultText: 'Select',
                    dynamicButtonTextSuffix: 'checked'
                };

                $scope.searchFilter = $scope.searchFilter || '';

                if (angular.isDefined($scope.settings.groupBy)) {
                    $scope.$watch('options', function (newValue) {
                        if (angular.isDefined(newValue)) {
                            $scope.orderedItems = $filter('orderBy')(newValue, $scope.settings.groupBy);
                        }
                    });
                }

                angular.extend($scope.settings, $scope.extraSettings || []);
                angular.extend($scope.externalEvents, $scope.events || []);
                angular.extend($scope.texts, $scope.translationTexts);

                $scope.singleSelection = $scope.settings.selectionLimit === 1;

                function getFindObj(id) {
                    var findObj = {};

                    if ($scope.settings.externalIdProp === '') {
                        findObj[$scope.settings.idProp] = id;
                    } else {
                        findObj[$scope.settings.externalIdProp] = id;
                    }

                    return findObj;
                }

                function clearObject(object) {
                    for (var prop in object) {
                        delete object[prop];
                    }
                }

                if ($scope.singleSelection) {
                    ;
                    alert('singleSelection');
                    if (angular.isArray($scope.selectedModel) && $scope.selectedModel.length === 0) {
                        clearObject($scope.selectedModel);
                    }
                }

                if ($scope.settings.closeOnBlur) {
                    $document.on('click', function (e) {
                        var target = e.target.parentElement;
                        var parentFound = false;

                        while (angular.isDefined(target) && target !== null && !parentFound) {
                            if (_.contains(target.className.split(' '), 'multiselect-parent') && !parentFound) {
                                if (target === $dropdownTrigger) {
                                    parentFound = true;
                                }
                            }
                            target = target.parentElement;
                        }

                        if (!parentFound) {
                            $scope.$apply(function () {
                                $scope.open = false;
                            });
                        }
                    });
                }

                $scope.getGroupTitle = function (groupValue) {
                    if ($scope.settings.groupByTextProvider !== null) {
                        return $scope.settings.groupByTextProvider(groupValue);
                    }

                    return groupValue;
                };

                $scope.getButtonText = function () {
                    if ($scope.settings.dynamicTitle && ($scope.selectedModel.length > 0 || (angular.isObject($scope.selectedModel) && _.keys($scope.selectedModel).length > 0))) {
                        if ($scope.settings.smartButtonMaxItems > 0) {
                            var itemsText = [];

                            angular.forEach($scope.options, function (optionItem) {
                                if ($scope.isChecked($scope.getPropertyForObject(optionItem, $scope.settings.idProp))) {
                                    var displayText = $scope.getPropertyForObject(optionItem, $scope.settings.displayProp);
                                    var converterResponse = $scope.settings.smartButtonTextConverter(displayText, optionItem);

                                    itemsText.push(converterResponse ? converterResponse : displayText);
                                }
                            });

                            if ($scope.selectedModel.length > $scope.settings.smartButtonMaxItems) {
                                itemsText = itemsText.slice(0, $scope.settings.smartButtonMaxItems);
                                itemsText.push('...');
                            }

                            return itemsText.join(', ');
                        } else {
                            var totalSelected;

                            if ($scope.singleSelection) {
                                totalSelected = ($scope.selectedModel !== null && angular.isDefined($scope.selectedModel[$scope.settings.idProp])) ? 1 : 0;
                            } else {
                                totalSelected = angular.isDefined($scope.selectedModel) ? $scope.selectedModel.length : 0;
                            }

                            if (totalSelected === 0) {
                                return $scope.texts.buttonDefaultText;
                            } else {
                                return totalSelected + ' ' + $scope.texts.dynamicButtonTextSuffix;
                            }
                        }
                    } else {
                        return $scope.texts.buttonDefaultText;
                    }
                };

                $scope.getPropertyForObject = function (object, property) {
                    if (angular.isDefined(object) && object.hasOwnProperty(property)) {
                        return object[property];
                    }

                    return '';
                };

                $scope.selectAll = function () {
                    // alert('selectAll');
                    $scope.deselectAll(false);
                    $scope.externalEvents.onSelectAll();

                    angular.forEach($scope.options, function (value) {
                        $scope.setSelectedItem(value[$scope.settings.idProp], true);
                    });
                };

                $scope.deselectAll = function (sendEvent) {

                    // alert('deselectAll');
                    sendEvent = sendEvent || true;

                    if (sendEvent) {
                        $scope.externalEvents.onDeselectAll();
                    }
                    $scope.KOTS = "";
                    $scope.tableid = "";
                    if ($scope.singleSelection) {
                        clearObject($scope.selectedModel);
                    } else {

                        $scope.selectedModel.splice(0, $scope.selectedModel.length);
                    }
                };


                $scope.setSelectedItem = function (id, dontRemove) {


                    var Kots = {};
                    var kot = id.split("-")[1];
                    Kots = kot;
                    var findObj = getFindObj(id);
                    ;
                    var finalObj = null;

                    if ($scope.settings.externalIdProp === '') {
                        finalObj = _.find($scope.options, findObj);
                    } else {
                        finalObj = findObj;
                    }

                    if ($scope.singleSelection) {
                        // alert('singleSelection1');
                        clearObject($scope.selectedModel);
                        angular.extend($scope.selectedModel, finalObj);
                        $scope.externalEvents.onItemSelect(finalObj);
                        if ($scope.settings.closeOnSelect) $scope.open = false;

                        return;
                    }

                    dontRemove = dontRemove || false;

                    var exists = _.findIndex($scope.selectedModel, findObj) !== -1;


                    if (!dontRemove && exists) {

                        $scope.selectedModel.splice(_.findIndex($scope.selectedModel, findObj), 1);

                        $scope.KOTS = $scope.KOTS.replace(finalObj.id.split("-")[1] + ",", "");
                        // $scope.KOTS = $scope.KOTS.replace(/\,$/, '');  
                        $scope.tableid = $scope.KOTS.slice(0, -1);
                        //  $scope.tableid = $scope.KOTS;
                        $scope.externalEvents.onItemDeselect(findObj);
                    } else if (!exists && ($scope.settings.selectionLimit === 0 || $scope.selectedModel.length < $scope.settings.selectionLimit)) {

                        $scope.selectedModel.push(finalObj);


                        if (angular.isUndefined($scope.KOTS)) {
                            $scope.KOTS = "";
                        }

                        $scope.KOTS = $scope.KOTS + finalObj.id.split("-")[1] + ",";

                        $scope.tableid = $scope.KOTS.slice(0, -1);
                        $scope.externalEvents.onItemSelect(finalObj);
                    }
                    if ($scope.settings.closeOnSelect) $scope.open = false;
                };

                $scope.isChecked = function (id) {
                    ;

                    if ($scope.singleSelection) {
                        ;
                        // alert('singleSelection2');
                        return $scope.selectedModel !== null && angular.isDefined($scope.selectedModel[$scope.settings.idProp]) && $scope.selectedModel[$scope.settings.idProp] === getFindObj(id)[$scope.settings.idProp];
                    }

                    return _.findIndex($scope.selectedModel, getFindObj(id)) !== -1;
                };

                $scope.externalEvents.onInitDone();
            }
        };
    }]);


app.directive('multiselectdropdown1', function () {
    return {
        restrict: 'E',
        scope: {
            model: '=',
            multiselectoptions: '=',
            maxlenghttoshowselectedvalues: '=',
            onchangeeventofcheckbox: '&'
        },
        template:
        '<div  class="btn-group" ng-class={open:open}> \
            <button type="button" class="multiselect dropdown-toggle btn btn-default" title="None selected" ng-click="toggledropdown()" > \
                <span class="multiselect-selected-text">{{model.toggletext}}</span> \
                <b class="caret"></b> \
            </button> \
            <ul class="multiselect-container dropdown-menu"> \
                <li class="multiselect-item filter" value="0"> \
                    <div class="input-group"> \
                        <span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span> \
                        <input class="form-control multiselect-search" type="text" placeholder="Search" ng-model="model.query"><span class="input-group-btn"> \
                            <button class="btn btn-default multiselect-clear-filter" ng-click="clearsearch()" type="button"><i class="glyphicon glyphicon-remove-circle"></i></button> \
                        </span> \
                    </div> \
                </li> \
                 \
                <li ng-repeat="option in (filteredOptions = (multiselectoptions| filter:model.query))"><label style="padding: 3px 20px 3px 40px;margin-top:0px;margin-bottom:0px" class="checkbox"><input type="checkbox" ng-checked="isselected(option)"  ng-disabled="!checked" ng-model="option.Selected" ng-change="toggleselecteditem(option);doOnChangeOfCheckBox()">{{option.text}}</label></li> \
            </ul> \
        </div>',
        link: function ($scope, element, attr) {
            angular.element(document).ready(function () {
                ;
                $scope.toggledropdown = function () {
                    ;
                    $scope.open = !$scope.open;
                };

                $scope.toggleselectall = function ($event) {
                    var selectallclicked = true;
                    if ($scope.model.selectall == false) {
                        selectallclicked = false;
                    }
                    $scope.doonselectallclick(selectallclicked, $scope.filteredOptions);
                };

                $scope.doonselectallclick = function (selectallclicked, optionArrayList) {
                    ;
                    $scope.model = [];
                    if (selectallclicked) {
                        angular.forEach(optionArrayList, function (item, index) {
                            item["Selected"] = true;
                            $scope.model.push(item);
                        });

                        if (optionArrayList.length == $scope.multiselectoptions.length) {
                            $scope.model.selectall = true;
                        }
                    }
                    else {
                        angular.forEach(optionArrayList, function (item, index) {
                            item["Selected"] = false;
                        });
                        $scope.model.selectall = false;
                    }
                    $scope.settoggletext();
                }

                $scope.toggleselecteditem = function (option) {
                    ;
                    var intIndex = -1;
                    angular.forEach($scope.model, function (item, index) {
                        if (item.value == option.value) {
                            intIndex = index;
                        }
                    });

                    if (intIndex >= 0) {
                        $scope.model.splice(intIndex, 1);
                    }
                    else {
                        $scope.model.push(option);
                    }

                    if ($scope.model.length == $scope.multiselectoptions.length) {
                        $scope.model.selectall = true;
                    }
                    else {
                        $scope.model.selectall = false;
                    }
                    $scope.settoggletext();
                };

                $scope.clearsearch = function () {
                    $scope.model.query = "";
                }

                $scope.settoggletext = function () {
                    ;
                    if ($scope.model.length > $scope.maxlenghttoshowselectedvalues) {
                        $scope.model.toggletext = $scope.model.length + " Selected";

                    }
                    else {
                        $scope.model.toggletext = "";
                        angular.forEach($scope.model, function (item, index) {
                            if (index == 0) {
                                $scope.model.toggletext = item.text;
                            }
                            else {
                                $scope.model.toggletext += ", " + item.text;
                            }
                        });

                        if (!($scope.model.toggletext.length > 0)) {
                            $scope.model.toggletext = "None Selected"
                        }
                    }
                }

                $scope.isselected = function (option) {
                    ;
                    var selected = false;


                    angular.forEach($scope.model, function (item, index) {


                        if (item.value == option.value) {

                            alert(item.value);
                            selected = true;
                            //$scope.checked = true;
                            $scope.settoggletext();
                        }
                    });
                    option.Selected = selected;
                    return selected;
                }


                $scope.doOnChangeOfCheckBox = function () {
                    $scope.onchangeeventofcheckbox();
                }();
            });
        }
    }
});

app.directive('multiselectdropdown', function () {
    return {
        restrict: 'EA',
        scope: {
            model: '=',
            multiselectoptions: '=',
            maxlenghttoshowselectedvalues: '=',
            onchangeeventofcheckbox: '&'
        },
        template:
        '<div  class="btn-group" ng-class={open:open}> \
            <button type="button" class="multiselect dropdown-toggle btn btn-default" title="None selected" ng-click="toggledropdown()" > \
                <span class="multiselect-selected-text">{{model.toggletext}}</span> \
                <b class="caret"></b> \
            </button> \
            <ul class="multiselect-container dropdown-menu"> \
                <li class="multiselect-item filter" value="0"> \
                    <div class="input-group"> \
                        <span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span> \
                        <input class="form-control multiselect-search" type="text" placeholder="Search" ng-model="model.query"><span class="input-group-btn"> \
                            <button class="btn btn-default multiselect-clear-filter" ng-click="clearsearch()" type="button"><i class="glyphicon glyphicon-remove-circle"></i></button> \
                        </span> \
                    </div> \
                </li> \
                <li class="multiselect-item multiselect-all"><label style="padding: 3px 20px 3px 40px;margin-top:0px;margin-bottom:0px" class="checkbox"><input type="checkbox" ng-model="model.selectall" ng-change="toggleselectall()">Select all</label></li> \
                <li ng-repeat="option in (filteredOptions = (multiselectoptions| filter:model.query))"><label style="padding: 3px 20px 3px 40px;margin-top:0px;margin-bottom:0px" class="checkbox"><input type="checkbox" ng-checked="isselected(option)" ng-model="option.Selected" ng-change="toggleselecteditem(option);doOnChangeOfCheckBox()">{{option.text}}</label></li> \
            </ul> \
        </div>',
        link: function ($scope, element, attr) {
            angular.element(document).ready(function () {
                ;
                $scope.toggledropdown = function () {
                    ;
                    $scope.open = !$scope.open;
                };

                $scope.toggleselectall = function ($event) {
                    ;
                    var selectallclicked = true;
                    if ($scope.model.selectall == false) {
                        selectallclicked = false;
                    }
                    $scope.doonselectallclick(selectallclicked, $scope.filteredOptions);
                };

                $scope.doonselectallclick = function (selectallclicked, optionArrayList) {
                    ;
                    $scope.model = [];
                    if (selectallclicked) {
                        angular.forEach(optionArrayList, function (item, index) {
                            item["Selected"] = true;
                            $scope.model.push(item);
                        });

                        if (optionArrayList.length == $scope.multiselectoptions.length) {
                            $scope.model.selectall = true;
                        }
                    }
                    else {
                        angular.forEach(optionArrayList, function (item, index) {
                            item["Selected"] = false;
                        });
                        $scope.model.selectall = false;
                    }
                    $scope.settoggletext();
                }

                $scope.toggleselecteditem = function (option) {
                    ;
                    var intIndex = -1;

                    // alert(option.value);
                    angular.forEach($scope.model, function (item, index) {
                        if (item.value == option.value) {
                            intIndex = index;
                        }
                    });

                    if (intIndex >= 0) {
                        $scope.model.splice(intIndex, 1);
                    }
                    else {
                        $scope.model.push(option);
                    }

                    if ($scope.model.length == $scope.multiselectoptions.length) {
                        $scope.model.selectall = true;
                    }
                    else {
                        $scope.model.selectall = false;
                    }
                    $scope.settoggletext();
                };

                $scope.clearsearch = function () {
                    $scope.model.query = "";
                }

                $scope.settoggletext = function () {
                    ;
                    if ($scope.model.length > $scope.maxlenghttoshowselectedvalues) {
                        $scope.model.toggletext = $scope.model.length + " Selected";
                    }
                    else {
                        $scope.model.toggletext = "";
                        angular.forEach($scope.model, function (item, index) {
                            if (index == 0) {
                                $scope.model.toggletext = item.text;
                            }
                            else {
                                $scope.model.toggletext += ", " + item.text;
                            }
                        });

                        if (!($scope.model.toggletext.length > 0)) {
                            $scope.model.toggletext = "None Selected"
                        }
                    }
                }

                $scope.isselected = function (option) {
                    ;
                    var selected = false;
                    if (option["selectall"] == false) {
                        $scope.doonselectallclick(false, option);
                        delete option["selectall"];
                    }
                    angular.forEach($scope.model, function (item, index) {
                        if (item.value == option.value) {
                            selected = true;
                            $scope.settoggletext();
                        }
                    });
                    option.Selected = selected;
                    return selected;
                }

                $scope.doOnChangeOfCheckBox = function () {
                    ;
                    $scope.onchangeeventofcheckbox();
                }();
            });
        }
    }
});

app.directive('kitQuantity', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {

            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (val) {
                debugger;
                var length = parseInt(element[0].maxLength) - 3;
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var clean = val.replace(/[^0-9\.]/g, '');
                var decimalCheck = clean.split('.');

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 3);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }
                if (decimalCheck.length == 1) {
                    if (decimalCheck[0].length > length) {
                        clean = decimalCheck[0].substring(0, length);
                    }
                }
                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });
            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
            element.bind('blur', function (event) {

                var el = element[0].value.split('.');
                var fval = parseFloat(element[0].value);
                if (fval <= 0) {
                    fval = null;
                }
                else if (el.length == 2) {
                    if (el[1] == "") {
                        fval = el[0];
                    }
                }

                ngModelCtrl.$setViewValue(fval);
                ngModelCtrl.$render();
                scope.$apply();
            });
        }

    };
}); // Quantity Format 
app.directive('kitAmount', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            ;
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (val) {
                
                if (val == "") { val = "0"; }
                var length = parseInt(element[0].maxLength) - 3;
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var clean = val.replace(/[^0-9\.]/g, '');
                var decimalCheck = clean.split('.');

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }
                if (decimalCheck.length == 1) {
                    if (decimalCheck[0].length > length) {
                        clean = decimalCheck[0].substring(0, length);
                    }
                }
                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });
            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
            element.bind('blur', function (event) {

                var el = element[0].value.split('.');
                var fval = parseFloat(element[0].value);
                if (fval <= 0) {
                    fval = null;
                }
                else if (el.length == 2) {
                    if (el[1] == "") {
                        fval = el[0];
                    }
                }

                ngModelCtrl.$setViewValue(fval);
                ngModelCtrl.$render();
                scope.$apply();
            });
        }

    };
}); // Amount Format 
app.directive('kitDigits', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (text) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var transformedInput = text.replace(/[^0-9]/g, '');
                if (transformedInput !== text) {
                    ngModelCtrl.$setViewValue(transformedInput);
                    ngModelCtrl.$render();
                }

                return transformedInput;
            });



        }

    };
}); //Only Digits kit-digits


app.directive('kitLandline', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (text) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var transformedInput = text.replace(/[^0-9\- ]/g, '');
                if (transformedInput !== text) {
                    ngModelCtrl.$setViewValue(transformedInput);
                    ngModelCtrl.$render();
                }

                return transformedInput;
            });



        }

    };
}); //Only Digits,space and hyphen kit-landline
app.directive('kitDate', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (text) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var transformedInput = text.replace(/[^0-9/-]/g, '');
                if (transformedInput !== text) {
                    ngModelCtrl.$setViewValue(transformedInput);
                    ngModelCtrl.$render();
                }

                return transformedInput;
            });
        }

    };
}); // Only Date 
app.directive('kitAlphabets', function () {
    return {
        require: '?ngModel',

        link: function (scope, element, attr, ngModelCtrl) {
            ngModelCtrl.$parsers.push(function (text) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var transformedInput = text.replace(/[^a-zA-Z ]/g, '');
                if (transformedInput !== text) {
                    ngModelCtrl.$setViewValue(transformedInput);
                    ngModelCtrl.$render();
                }

                return transformedInput;
            });
        }

    };
}); // Only Alphabets  
app.directive('kitAlphaNumeric', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            ;
            ngModelCtrl.$parsers.push(function (text) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var transformedInput = text.replace(/[^a-zA-Z0-9 ]/g, '');
                if (transformedInput !== text) {
                    ngModelCtrl.$setViewValue(transformedInput);
                    ngModelCtrl.$render();
                }
                return transformedInput;
            });
        }
    };
}); // Only AlphaNumeric  
app.directive('capitalize', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            //ngModelCtrl.$parsers.push(function (inputValue) {

            //    //if (inputValue == undefined) inputValue = '';
            //    //var capitalized = inputValue.toUpperCase();

            //    ////if (capitalized !== inputValue) {
            //    ////    ngModelCtrl.$setViewValue(capitalized);
            //    ////    ngModelCtrl.$render();
            //    ////}
            //    //return capitalized;
            //});
            ngModelCtrl.$parsers.push(function (input) {
                return input ? input.toUpperCase() : "";
            });
            element.css("text-transform", "uppercase");
        }
    };
}); // For Converting Text into Capital Letters 
app.directive('kitAlphaNumericwithSlash', function () {
    return {
        require: '?ngModel',

        link: function (scope, element, attr, ngModelCtrl) {
            ngModelCtrl.$parsers.push(function (text) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var transformedInput = text.replace(/[^a-zA-Z0-9/ ]/g, '');
                if (transformedInput !== text) {
                    ngModelCtrl.$setViewValue(transformedInput);
                    ngModelCtrl.$render();
                }

                return transformedInput;
            });
        }
    };

}); //  AlphaNumeric with Slash 

app.directive('kitAlphaNumericwithDot', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (val) {

                var length = parseInt(element[0].maxLength) - 3;
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var clean = val.replace(/[^a-zA-Z0-9\.]/g, '');
                var decimalCheck = clean.split('.');

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }
                if (decimalCheck.length == 1) {
                    if (decimalCheck[0].length > length) {
                        clean = decimalCheck[0].substring(0, length);
                    }
                }
                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });
            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
            element.bind('blur', function (event) {

                var el = element[0].value.split('.');
                var fval = parseFloat(element[0].value);
                if (fval <= 0) {
                    fval = null;
                }
                else if (el.length == 2) {
                    if (el[1] == "") {
                        fval = el[0];
                    }
                }
                ngModelCtrl.$setViewValue(fval);
                ngModelCtrl.$render();
            });
        }

    };
}); // Amount Format 

app.directive('kitLiterQty', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var clean = val.replace(/[^-0-9\.]/g, '');
                var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                if (!angular.isUndefined(negativeCheck[1])) {
                    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                    clean = negativeCheck[0] + '-' + negativeCheck[1];
                    if (negativeCheck[0].length > 0) {
                        clean = negativeCheck[0];
                    }
                }
                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 3);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }
                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }

                return clean;
            });
            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
    };
}); // 0.758 format numbers only
app.directive('kitIp', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (text) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }

                // var transformedInput = text.replace(/[^0-9.]/g, '');

                var transformedInput = text.replace(/ ^(?:[1-9]\d*|0)$/g, '');
                if (transformedInput !== text) {
                    ngModelCtrl.$setViewValue(transformedInput);
                    ngModelCtrl.$render();
                }

                return transformedInput;
            });
        }

    };
}); //Only Digits kit-digits

app.filter('sri', function () {
    return function (name) {

        var s = name.lengrh;
        var res = name.split(" ");
        if (res.length == 1) {
            return name.substring(0, 3);
        } else {
            var matches = name.match(/\b(\w)/g);
            return matches.join('');
        }
    }
})

function CheckPercentage(evt) {

    var NumValue = evt.target || evt.srcElement;
    var length = NumValue.value.length;
    var parts = NumValue.value.split('.');
    var keyCode = evt.keyCode ? evt.keyCode : ((evt.charCode) ? evt.charCode : evt.which);
    if (!(keyCode >= 48 && keyCode <= 57)) {
        if (!(keyCode == 8 || keyCode == 9 || keyCode == 35 || keyCode == 36 || keyCode == 37 || keyCode == 39 || keyCode == 46)) {
            return false;
        }
        else if (keyCode == 46) {
            if ((NumValue.value) && (NumValue.value.indexOf('.') >= 0))
                return false;
            else
                return true;
        }
    }

    if (parts[0].length == 2 && length == 2) { return false; }
    else {

        if (parts[0].length >= 14) return false;
        if (parts.length == 2 && parts[1].length >= 2) return false;
    }
} // Percentage Validation  
function convertToDate(value) {

    if (value.length != 0) {
        var arr = [];

        arr = value.split('-');
        if (arr.length == 1) {
            arr = value.split('/');
        }
        return arr[0] + '/' + arr[1] + '/' + arr[2].substring(0, 4);


    }
} // Convert To Date  
function changedate(value) {
    debugger
    if (value.length != 0) {
        var arr = [];

        arr = value.split('-');
        if (arr.length == 1) {
            arr = value.split('/');
        }
        return arr[1] + '/' + arr[0] + '/' + arr[2].substring(0, 4);


    }
}
function calculate_age(birth_day, birth_month, birth_year) {
    today_date = new Date();
    today_year = today_date.getFullYear();
    today_month = today_date.getMonth();
    today_day = today_date.getDate();
    age = today_year - birth_year;

    if (today_month < (birth_month - 1)) {
        age--;
    }
    if (((birth_month - 1) == today_month) && (today_day < birth_day)) {
        age--;
    }
    return age;
} // Caluculate Age 
function CheckDate(startDate, endDate, Msg) {

    var stdt = $('#' + startDate.id + '').val();
    var eddt = $('#' + endDate.id + '').val();
    if (stdt == "") {
        $('#' + startDate.id + '').val('');
        $('#' + startDate.id + '').focus();
        return false;
    } else if (eddt == "") {
        $('#' + endDate.id + '').val('');
        $('#' + endDate.id + '').focus();
        return false;
    }
    else {
        var arrjd = $('#' + startDate.id + '').val().split('/');
        var arrrd = $('#' + endDate.id + '').val().split('/');

        var SD = new Date(arrjd[1] + '/' + arrjd[0] + '/' + arrjd[2]);
        var ED = new Date(arrrd[1] + '/' + arrrd[0] + '/' + arrrd[2]);

        if (SD > ED) {

            alert(Msg);
            $('#' + endDate.id + '').val('');
            $('#' + endDate.id + '').focus();
            var date = new Date().toLocaleDateString("en-au", { year: "numeric", month: "numeric", day: "numeric" }).replace(/\s/g, '/');

            // $('#' + endDate.id + '').datepicker("setDate", date);
            $('#' + startDate.id + '').datepicker("setDate", "today");
            $('#' + endDate.id + '').datepicker("setDate", "today");
            $('#' + startDate.id + '').val('');
            $('#' + endDate.id + '').val('');

            return false;
        }

        return true;
    }
} // Date Range  
function JqCustomSearch(searchVal, grid) {

    var rules = [], i, cm,

postData = grid.jqGrid("getGridParam", "postData"),
colModel = grid.jqGrid("getGridParam", "colModel"),
searchText = searchVal,
l = colModel.length;

    for (i = 0; i < l; i++) {
        cm = colModel[i];
        if (cm.search !== false && (typeof cm.stype === "undefined" || cm.stype === "text")) {
            rules.push({
                field: cm.name,
                op: "cn",
                data: searchText
            });
        }
    }

    $.extend(postData, {
        filters: {
            groupOp: "OR",
            rules: rules
        }
    });

    grid.jqGrid("setGridParam", { search: true, postData: postData });
    grid.trigger("reloadGrid", [{ page: 1, current: true }]);
    return false;
}
function FnNotNull(x) {
    var res = 0;
    if (angular.isDefined(x) && x != "") {
        return parseInt(x);
    }
    else {
        return 0;
    }

}


