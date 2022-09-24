import { FileStatusEnum } from "./fileStatusEnum";

export class FileState {
    name: string;
    progress: number;
    status: FileStatusEnum;

    constructor(name: string) {
        this.name = name;
        this.progress = 0;
        this.status = FileStatusEnum.Pending;
    }
}