import * as vscode from 'vscode';
import { taskProviderType } from './constants';
import { Configuration } from './configuration';


export class DotNetTaskProvider implements vscode.TaskProvider {
    provideTasks(token: vscode.CancellationToken): vscode.ProviderResult<vscode.Task[]> {
        return [ DotNetBuildTask.create() ];
    }
    resolveTask(task: vscode.Task, token: vscode.CancellationToken): vscode.ProviderResult<vscode.Task> {
        return DotNetBuildTask.create();
    }
}

class DotNetTaskDefinition implements vscode.TaskDefinition {
    public name: string = "dotnet-meteor build";
    public type: string = taskProviderType;
}

class DotNetBuildTask {
    public static readonly title: string = "Build";
    public static readonly source: string = "dotnet-meteor";

    private static readonly emptyTask: vscode.Task = new vscode.Task(
        new DotNetTaskDefinition(), vscode.TaskScope.Workspace, DotNetBuildTask.title, DotNetBuildTask.source
    );

    public static create(): vscode.Task {
        if (!DotNetBuildTask.validateParameters())
            return DotNetBuildTask.emptyTask;
    
        const devicePlatform = Configuration.selectedDevice!.platform;
        const framework = Configuration.selectedProject?.frameworks?.find(it => it.includes(devicePlatform!));
        
        if (!framework) {
            vscode.window.showErrorMessage(`No framework for ${devicePlatform} found`);
            return DotNetBuildTask.emptyTask;
        }

        return new vscode.Task(
            new DotNetTaskDefinition(),
            vscode.TaskScope.Workspace,
            DotNetBuildTask.title,
            DotNetBuildTask.source,
            new vscode.ShellExecution(
                `dotnet build ${Configuration.selectedProject!.path} -c:${Configuration.selectedTarget!} -f:${framework}`
            )
        );
    }

    private static validateParameters(): boolean {
        if (!Configuration.selectedDevice) {
            vscode.window.showErrorMessage('No device selected');
            return false;
        }

        if (!Configuration.selectedDevice.platform) {
            vscode.window.showErrorMessage('Device platform not specified');
            return false;
        }

        if (!Configuration.selectedProject?.path) {
            vscode.window.showErrorMessage('Selected project not found');
            return false;
        }

        if (!Configuration.selectedTarget) {
            vscode.window.showErrorMessage('No configuration selected');
            return false;
        }

        return true;
    }
}