﻿/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using System.Linq;

namespace NPOI.SS.Formula.Functions
{
    /**
     * Implementation for the ERROR.TYPE() Excel function.
     * <p>
     * <b>Syntax:</b><br/>
     * <b>ERROR.TYPE</b>(<b>errorValue</b>)</p>
     * <p>
     * Returns a number corresponding to the error type of the supplied argument.</p>
     * <p>
     *    <table border="1" cellpadding="1" cellspacing="1" summary="Return values for ERROR.TYPE()">
     *      <tr><td>errorValue</td><td>Return Value</td></tr>
     *      <tr><td>#NULL!</td><td>1</td></tr>
     *      <tr><td>#DIV/0!</td><td>2</td></tr>
     *      <tr><td>#VALUE!</td><td>3</td></tr>
     *      <tr><td>#REF!</td><td>4</td></tr>
     *      <tr><td>#NAME?</td><td>5</td></tr>
     *      <tr><td>#NUM!</td><td>6</td></tr>
     *      <tr><td>#N/A!</td><td>7</td></tr>
     *      <tr><td>everything else</td><td>#N/A!</td></tr>
     *    </table>
     *
     * Note - the results of ERROR.TYPE() are different to the constants defined in
     * <tt>ErrorConstants</tt>.
     * </p>
     *
     * @author Josh Micich
     */

    public class Errortype : Fixed1ArgFunction
    {

        public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
        {

            try
            {
                OperandResolver.GetSingleValue(arg0, srcRowIndex, srcColumnIndex);
                return ErrorEval.NA;
            }
            catch (EvaluationException e)
            {
                int result = TranslateErrorCodeToErrorTypeValue(e.GetErrorEval().ErrorCode);
                return new NumberEval(result);
            }
        }

        private int TranslateErrorCodeToErrorTypeValue(int errorCode)
        {
            var values = new FormulaError[] {
                FormulaError.NULL,
                FormulaError.DIV0,
                FormulaError.VALUE,
                FormulaError.REF,
                FormulaError.NAME,
                FormulaError.NUM,
                FormulaError.NA,
                FormulaError.CIRCULAR_REF,
                FormulaError.FUNCTION_NOT_IMPLEMENTED
            };

            try
            {
                var error = FormulaError.ForInt(errorCode);
                return values.ToList()
                    // add index to list
                    .Select((item, index) => new { index, item })
                    // find error in list of errors
                    .Where(i => i.item == error)
                    // zero based list so add 1
                    .FirstOrDefault().index + 1;
            }
            catch { }
            throw new ArgumentException("Invalid error code (" + errorCode + ")");
        }

    }

}