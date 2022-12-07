﻿using System.Net;
using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities;

//This class will be called via Reflection by the generated REST API controller class
// ReSharper disable once UnusedType.Global
public static class RequestHandler {
    public static string Handle(string methodName, Dictionary<string, object> variables) {
        Console.WriteLine($"Called RequestHandler.Handle for {methodName}");
        var m = Global.CurrentServices
            .SelectMany(c => c.Methods)
            .First(t => t.Method.MethodName == methodName);

        //Check the WhenConditions to see whether the request fulfills the conditions
        var canContinue = CheckWhenConditions(m, variables);
        if (!canContinue)
            throw new RestHttpException {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Error = "BeeRock error. Unable to match \"When\" conditions.  Please re-check."
            };


        //throws a custom exception that will be handled by the middleware
        if ((int)m.SelectedHttpResponseType.StatusCode >= 400)
            throw new RestHttpException {
                StatusCode = m.SelectedHttpResponseType.StatusCode,
                Error = Helper.RemoveComments(m.ResponseText)
            };

        //200 OK
        return ScriptedJson.Evaluate(m.ResponseText, variables);
    }

    private static bool CheckWhenConditions(ServiceMethodItem serviceMethodItem, Dictionary<string, object> variables) {
        foreach (var condition in serviceMethodItem.WhenConditions.Where(w => w.IsActive)) {
            var result = PyEngine.Evaluate(condition.BoolExpression, variables);
            if (!(bool)result) return false;
        }

        return true;
    }
}
