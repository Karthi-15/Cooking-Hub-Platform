import { CookingClass } from "./cooking-class.model";
import { User } from "./user.model";

export interface CookingClassRequest { 
    cookingClassRequestId?: number;   
    userId: number; 
    cookingClassId: number;  
    requestDate: string; 
    status: string;  
    dietaryPreferences: string; 
    cookingGoals: string;  
    comments?: string;  
    cookingClass? : CookingClass;
    user?: User
}