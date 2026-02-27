export interface Feedback {
    feedbackId?: number;
    userId: number;
    username:string;
    email:string;
    feedbackText: string;
    date: Date;
}