Ext.ns('MyCompany');
My = MyCompany;
if (!My) {
    My = {};
}

My.makeWebServiceCall = function(url, params, resultCallback, config) {
    var obj;
    if (Ext.isString(url)) {
        obj = { url: url };
    } else {
        obj = url;
    }
    params = params || {};

    Ext.applyIf(obj, {
        disableCaching: true,
        params: params,
        callback: My.webServiceResultReceived,
        webServiceResultCallback: resultCallback,
        customConfig: config
    });

    Ext.Ajax.request(obj);
}

My.webServiceResultReceived = function(o, success, response) {
    var config = o.customConfig || {};
    var params = o.params;

    var result = (response.responseXML && response.responseXML.documentElement) ? response.responseXML : response.responseText;
    result = My.parseWebServiceResult(result, config);

    var isValid = !(typeof (result) == 'string');

    //The callback was stored in the request object itself.
    o.webServiceResultCallback(isValid, result, params, config.additionalCallbackParams);
}

My.parseWebServiceResult = function(result, config) {
    if (Ext.isEmpty(result)) {
        return (result);

    } else if (result.documentElement) {
        //Xml. Check for Xml should be before check for jsonResult.
        var documentElement = result.documentElement;
        if (documentElement.childNodes && documentElement.childNodes.length > 0) {
            //There is no childNodes[0], if server returns a null value.
            var node = documentElement.childNodes[0];
            var text = node ? Ext.DomQuery.selectValue('string', result) : null;

            var data = Ext.decode(text);
            if (data && data.Message && data.StackTrace) {
                return (data.Message);
            }
            else {
                return (data);
            }
        } else {
            return (null);
        }

    } else if (config.jsonResult) {
        if (Ext.isObject(result) || Ext.isArray(result)) {
            return (result);
        } else {
            try {
                return (Ext.decode(result));
            } catch (e) {
                return (result);
            }
        }

    } else if (Ext.isString(result)) {
        return (result);

    } else {
        var message = 'Unrecognized web service result';
        Ext.Msg.alert('Error', message);
    }
    //    if (Ext.isArray(result)) {
    //        return (result);
    //    } else if (result && result.Message && result.StackTrace) {
    //        return (result.Message);
    //    }
    //    else {
    //        var data = Ext.decode(result);
    //        return (data);
    //    }
}

var sequenceForReferenceRegistration = function(cls, methodName) {
    if (!methodName) methodName = 'addEvents';

    cls.prototype[methodName] = Ext.Function.createSequence(cls.prototype[methodName], function() {
        if (!this.nsReferenceRegistered) {
            if (this.ns) {
                var iid = this.getItemId ? this.getItemId() : this.itemId;
                if (iid) {
                    if (Ext.isFunction(this.ns.reg)) {
                        this.ns.reg(iid, this);
                    } else {
                        this.ns[iid] = this;
                    }
                }
            }

            this.nsReferenceRegistered = true;
        }
    });

}
sequenceForReferenceRegistration(Ext.AbstractComponent);
sequenceForReferenceRegistration(Ext.data.AbstractStore, 'setProxy');
