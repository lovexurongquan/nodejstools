﻿//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************//

using System.Collections.Generic;
using Microsoft.NodejsTools.Debugger.Serialization;
using Microsoft.VisualStudioTools.Project;
using Newtonsoft.Json.Linq;

namespace Microsoft.NodejsTools.Debugger.Commands {
    sealed class SetVariableValueCommand : DebuggerCommand {
        private readonly Dictionary<string, object> _arguments;
        private readonly string _name;
        private readonly IEvaluationResultFactory _resultFactory;
        private readonly NodeStackFrame _stackFrame;

        public SetVariableValueCommand(int id, IEvaluationResultFactory resultFactory, NodeStackFrame stackFrame, string name, int handle)
            : base(id, "setVariableValue") {
            Utilities.ArgumentNotNull("resultFactory", resultFactory);
            Utilities.ArgumentNotNull("stackFrame", stackFrame);
            Utilities.ArgumentNotNullOrEmpty("name", name);

            _resultFactory = resultFactory;
            _stackFrame = stackFrame;
            _name = name;

            _arguments = new Dictionary<string, object> {
                { "name", name },
                { "newValue", new { handle } },
                { "scope", new { frameNumber = stackFrame.FrameId, number = 0 } }
            };
        }

        protected override IDictionary<string, object> Arguments {
            get { return _arguments; }
        }

        public NodeEvaluationResult Result { get; private set; }

        public override void ProcessResponse(JObject response) {
            base.ProcessResponse(response);

            var variableProvider = new NodeSetValueVariable(_stackFrame, _name, response);
            Result = _resultFactory.Create(variableProvider);
        }
    }
}