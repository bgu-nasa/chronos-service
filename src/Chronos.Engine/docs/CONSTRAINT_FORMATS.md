# Constraint Format Reference

This document provides comprehensive documentation for all constraint types supported by the Chronos constraint evaluation engine.

## Overview

Constraints are stored in the `ActivityConstraint` table with a Key/Value structure:
- **Key**: Identifies the constraint type (e.g., `"preferred_weekdays"`)
- **Value**: Contains the constraint parameters (simple string or JSON format)

Each constraint is classified as either:
- **Hard Constraint**: Must be satisfied for assignment to be valid (violation = Error)
- **Soft Constraint**: Preference that should be satisfied when possible (violation = Warning)

## Supported Constraint Types

### 1. Preferred Weekdays

**Key**: `preferred_weekdays`  
**Type**: Soft Constraint  
**Value Format**: Comma-separated weekday names

**Description**: Specifies preferred weekdays for the activity. If the slot's weekday is not in the preferred list, a warning violation is returned.

**Examples**:
```
"Monday,Wednesday,Friday"
"Tuesday,Thursday"
"Monday"
```

**Validation**:
- Case-insensitive matching
- Whitespace is trimmed
- Empty value = no constraint

**Violation Message**: `"Slot weekday '{weekday}' is not in preferred weekdays: {list}"`

---

### 2. Time Range

**Key**: `time_range`  
**Type**: Hard Constraint  
**Value Format**: JSON object with `start` and `end` times in HH:mm format

**Description**: Specifies the allowed time range for the activity. The slot's time range must fall entirely within the specified range.

**Examples**:
```json
{"start": "08:00", "end": "17:00"}
{"start": "09:00", "end": "12:00"}
{"start": "13:00", "end": "18:00"}
```

**Validation**:
- Slot must start at or after `start` time
- Slot must end at or before `end` time
- Both `start` and `end` are required
- Times must be in HH:mm format (24-hour)

**Violation Message**: `"Slot time range ({from}-{to}) is outside allowed range ({start}-{end})"`

---

### 3. Required Capacity

**Key**: `required_capacity`  
**Type**: Hard Constraint  
**Value Format**: JSON object with optional `min` and/or `max` capacity

**Description**: Specifies capacity requirements for the resource. Also validates against `Activity.ExpectedStudents` if specified.

**Examples**:
```json
{"min": 30}
{"min": 20, "max": 50}
{"max": 100}
```

**Validation**:
- Resource must have capacity information
- If `min` is specified: `resource.Capacity >= min`
- If `max` is specified: `resource.Capacity <= max`
- If `Activity.ExpectedStudents` is set: `resource.Capacity >= ExpectedStudents`

**Violation Messages**:
- `"Resource capacity ({capacity}) is below minimum required ({min})"`
- `"Resource capacity ({capacity}) exceeds maximum allowed ({max})"`
- `"Resource capacity ({capacity}) is insufficient for expected students ({count})"`
- `"Resource '{identifier}' does not have capacity information"`

---

### 4. Location Preference

**Key**: `location_preference`  
**Type**: Soft Constraint  
**Value Format**: Comma-separated location names

**Description**: Specifies preferred locations for the activity. If the resource's location is not in the preferred list, a warning violation is returned.

**Examples**:
```
"Building A,Building B,Building C"
"Main Campus,North Campus"
"Engineering Building"
```

**Validation**:
- Case-insensitive matching
- Whitespace is trimmed
- Empty value = no constraint

**Violation Message**: `"Resource location '{location}' is not in preferred locations: {list}"`

---

### 5. Compatible Resource Types

**Key**: `compatible_resource_types`  
**Type**: Hard Constraint  
**Value Format**: Comma-separated resource type names

**Description**: Specifies which resource types are compatible with the activity type. The resource's type must be in the compatible list.

**Examples**:
```
"Lecture Hall,Seminar Room,Laboratory"
"Computer Lab,Engineering Lab"
"Auditorium"
```

**Validation**:
- Requires loading `ResourceType` from database
- Case-insensitive matching
- Whitespace is trimmed
- Empty value = error (at least one type must be specified)

**Violation Messages**:
- `"Resource type '{type}' is not compatible with activity type '{activityType}'"`
- `"Resource type information not found for resource '{identifier}'"`
- `"No compatible resource types specified"`

---

## Usage Examples

### Example 1: Lecture with Time and Capacity Constraints

```sql
INSERT INTO ActivityConstraint (ActivityId, OrganizationId, Key, Value)
VALUES 
  (@activityId, @orgId, 'time_range', '{"start": "08:00", "end": "17:00"}'),
  (@activityId, @orgId, 'required_capacity', '{"min": 50, "max": 200}'),
  (@activityId, @orgId, 'compatible_resource_types', 'Lecture Hall,Auditorium');
```

### Example 2: Lab Session with Location and Weekday Preferences

```sql
INSERT INTO ActivityConstraint (ActivityId, OrganizationId, Key, Value)
VALUES 
  (@activityId, @orgId, 'preferred_weekdays', 'Monday,Wednesday,Friday'),
  (@activityId, @orgId, 'location_preference', 'Engineering Building,Science Building'),
  (@activityId, @orgId, 'compatible_resource_types', 'Computer Lab,Engineering Lab'),
  (@activityId, @orgId, 'required_capacity', '{"min": 20, "max": 30}');
```

### Example 3: Seminar with Soft Constraints Only

```sql
INSERT INTO ActivityConstraint (ActivityId, OrganizationId, Key, Value)
VALUES 
  (@activityId, @orgId, 'preferred_weekdays', 'Tuesday,Thursday'),
  (@activityId, @orgId, 'location_preference', 'Main Campus');
```

---

## Adding New Constraint Types

To add a new constraint type:

1. **Create a Validator Class** implementing `IConstraintValidator`:
   ```csharp
   public class MyConstraintValidator : IConstraintValidator
   {
       public string ConstraintKey => "my_constraint_type";
       
       public async Task<ConstraintViolation?> ValidateAsync(
           ActivityConstraint constraint,
           Activity activity,
           Slot slot,
           Resource resource)
       {
           // Implementation
       }
   }
   ```

2. **Register in DI Container** (in `Program.cs`):
   ```csharp
   builder.Services.AddScoped<IConstraintValidator, MyConstraintValidator>();
   ```

3. **Document the Format** in this file

4. **Create Unit Tests** in `Chronos.Tests.Engine/Validators/`

5. **Update Integration Tests** to include the new constraint type

---

## Performance Characteristics

All constraint validators are designed to meet the <1ms evaluation requirement:

- **Simple validators** (preferred_weekdays, location_preference): ~0.1ms
- **JSON validators** (time_range, required_capacity): ~0.2ms
- **Database validators** (compatible_resource_types): ~0.5ms (with caching)

**Benchmark Results** (average over 100 iterations):
- Single constraint: ~0.15ms
- Five constraints: ~0.65ms
- All validators satisfy the <1ms requirement

---

## Error Handling

All validators implement comprehensive error handling:

1. **Malformed JSON**: Returns hard violation with error message
2. **Missing Required Fields**: Returns hard violation
3. **Invalid Values**: Returns hard violation
4. **Database Errors**: Returns hard violation with details
5. **Empty/Null Values**: Behavior depends on constraint type (documented above)

---

## Best Practices

1. **Use Hard Constraints Sparingly**: Too many hard constraints can make scheduling impossible
2. **Combine Hard and Soft**: Use hard constraints for requirements, soft for preferences
3. **Test Constraint Combinations**: Ensure constraints don't conflict
4. **Document Custom Constraints**: Always document new constraint types in this file
5. **Monitor Performance**: Use performance tests when adding new validators
