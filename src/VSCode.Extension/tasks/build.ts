import { ConfigurationController } from '../configuration';
import { ProcessArgumentBuilder } from '../bridge';
import * as res from '../resources';
import * as vscode from 'vscode';


export class DotNetTaskProvider implements vscode.TaskProvider {
    resolveTask(task: vscode.Task, token: vscode.CancellationToken): vscode.ProviderResult<vscode.Task> { 
        return this.getTask(task.definition)
    }
    provideTasks(token: vscode.CancellationToken): vscode.ProviderResult<vscode.Task[]> {
        return [
            this.getTask({ type: res.taskDefinitionId, target: res.taskDefinitionDefaultTarget })
        ];
    }

    private getTask(definition: vscode.TaskDefinition): vscode.Task {
        if (!definition.target)
            definition.target = res.taskDefinitionDefaultTarget;
    
        const builder = new ProcessArgumentBuilder('dotnet')
            .append(definition.target.toLowerCase());

        if (ConfigurationController.isActive()) {
            const framework = ConfigurationController.project!.frameworks
                .find(it => it.includes(ConfigurationController.device!.platform ?? 'undefined'))
            
            builder.appendQuoted(ConfigurationController.project!.path)
                .append(`-p:Configuration=${ConfigurationController.target}`)
                .append(`-p:TargetFramework=${framework}`);

            if (ConfigurationController.device!.runtime_id) {
                builder.append(`-p:RuntimeIdentifier=${ConfigurationController.device!.runtime_id}`);
            }
            if (ConfigurationController.isAndroid()) {
                builder.append('-p:EmbedAssembliesIntoApk=true');
                builder.append(`-p:AndroidSdkDirectory="${ConfigurationController.androidSdkDirectory}"`);
            }
            if (ConfigurationController.isWindows()) {
                builder.append('-p:WindowsPackageType=None');
                builder.append('-p:WinUISDKReferences=false');
            }
        }

        definition.args?.forEach((arg: string) => builder.override(arg));
        
        return new vscode.Task(
            definition, 
            vscode.TaskScope.Workspace, 
            definition.target, 
            res.extensionId,
            new vscode.ShellExecution(builder.build()),
            `$${res.taskProblemMatcherId}`
        );
    }
}