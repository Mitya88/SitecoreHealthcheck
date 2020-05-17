export class ComponentHealth{
    ErrorCount:number;
    Status:string;
    Name:string;
    Id:string;
    LastCheckTime:string;
    ErrorList:ErrorList;
    isLoading:boolean;
    HealthyMessage:string;
    Display:boolean;

}

export class ErrorList{
  Entries:Array<ErrorEntry>;
}

export class ErrorEntry{
    Reason:string;
    Created:string;
    Exception:any;
}

export class ComponentGroup{
    GroupName:string;
    Components:Array<ComponentHealth>;
}