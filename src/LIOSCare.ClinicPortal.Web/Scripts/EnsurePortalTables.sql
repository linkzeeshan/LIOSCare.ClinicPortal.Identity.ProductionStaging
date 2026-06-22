-- Ensure portal schema exists
CREATE SCHEMA IF NOT EXISTS portal;

-- Ensure all required portal tables exist
-- This script can be used to manually verify/create tables if migrations fail

-- Check if chat_sessions table exists
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'portal' AND table_name = 'chat_sessions'
    ) THEN
        RAISE NOTICE 'chat_sessions table does not exist in portal schema';
    ELSE
        RAISE NOTICE 'chat_sessions table exists in portal schema';
    END IF;
END $$;

-- List all portal tables
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'portal' 
ORDER BY table_name;

-- Check if specific tables exist
SELECT 
    'chat_sessions' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'chat_sessions') as exists
UNION ALL
SELECT 
    'chat_session_jobs' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'chat_session_jobs') as exists
UNION ALL
SELECT 
    'doctor_profiles' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'doctor_profiles') as exists
UNION ALL
SELECT 
    'patient_accounts' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'patient_accounts') as exists
UNION ALL
SELECT 
    'service_tiers' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'service_tiers') as exists
UNION ALL
SELECT 
    'clinics_hospitals' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'clinics_hospitals') as exists
UNION ALL
SELECT 
    'notifications' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'notifications') as exists
UNION ALL
SELECT 
    'chat_messages' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'chat_messages') as exists
UNION ALL
SELECT 
    'reschedule_requests' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'reschedule_requests') as exists
UNION ALL
SELECT 
    'session_reports' as table_name,
    EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'portal' AND table_name = 'session_reports') as exists;
