(async (window, undefined) => {
    // initialize event dispatcher and attach it to the 'message' window event to parse async responses from Host
    //http://using-d3js.com/08_02_dispatches.html

    var callbacks = {
        dynamic: function() {}
    };
    
    var dispatchEx = d3.dispatch("dynamic", "status");

    window.dispatch = d3.dispatch.apply(this, Object.keys(callbacks));

    for(var key in callbacks) {
        window.dispatch.on(key, callbacks[key]);
    }
    
    window.chrome.webview.addEventListener('message', function(e) {
        var response = e.data;
        window.dispatch.call(response.Callback, null, e, JSON.parse(response.JsonData));
    });
    
    var schema = {},
        validator = {},
        generator = {};

    function isFunction(functionToCheck) {
        return functionToCheck && {}.toString.call(functionToCheck) === '[object Function]';
    }

    function assertValidMessage() {

    }

    window.interop = {

        isInitialized: false,
        
        namespace: "Backsplashed.Interop.Commands",

        initialize: function () {
            schema = this.discoverCommands();
            validator = new djv();
            validator.addSchema("interop", schema);
            generator = new djvi();
            generator.addSchema("interop", schema);
            this.isInitialized = true;
        },

        createCommandInstance: function (type) {
            if (!this.isInitialized) {
                this.initialize();
            }

            var typeFullName = `${this.namespace}.${type}`;
            return generator.instance(`interop#/${typeFullName}`);
        },

        discoverCommands: function () {
            var externalSchemas =  window.chrome.webview.hostObjects.sync.backsplashedInterop.GenerateInteropSchema();
            return JSON.parse(externalSchemas);
        },

        sendCommand: function (type, data, callback) {
            if (!this.isInitialized) {
                this.initialize();
            }

            if (data == null) {
                data = {};
            }

            if (isFunction(callback)) {
                var dispatched = callback;
                var subtype = `dynamic.${uuidv4().replace(/-/g, "")}`;
                
                window.dispatch.on(subtype, function(e, data) {
                    dispatched(e, data);
                    window.dispatch.on(subtype, null);
                });

                callback = "dynamic";
            }

            var typeFullName = `${this.namespace}.${type}`;

            var result = validator.validate(`interop#/${typeFullName}`, data);
            if (result === undefined) {
                var message = {
                    type: typeFullName,
                    jsonData: JSON.stringify(data),
                    callback: callback
                };

                window.chrome.webview.postMessage(message);
            } else {
                console.error(`Data does not conform to the schema for 'interop#/${typeFullName}'`, result, schema[type]);
            }
        }
    };
})(window);